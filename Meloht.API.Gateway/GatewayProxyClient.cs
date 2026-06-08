using Meloht.API.Gateway.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace Meloht.API.Gateway
{
    public class GatewayProxyClient : IGatewayProxy
    {

        private readonly Channel<RequestModel> _channel;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly AppSettingsClient _appSettings;
        private int _requestTimeout;

        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<HttpResponseMessage>> _targetRequstQueue;
        private readonly ObjectPool<RequestModel> _requestModelPool;

        public GatewayProxyClient(IConfiguration configuration, IHttpClientFactory httpClientFactory, IHostApplicationLifetime lifetime)
        {
            _targetRequstQueue = new ConcurrentDictionary<Guid, TaskCompletionSource<HttpResponseMessage>>();
            _configuration = configuration;
            _appSettings = new AppSettingsClient(_configuration);
            _httpClient = httpClientFactory.CreateClient();
            _channel = Channel.CreateBounded<RequestModel>(GetChannelOptions(_appSettings.PoolSize));
            _requestTimeout = _appSettings.HttpRequestTimeout * 1000;
            _requestModelPool = new ObjectPool<RequestModel>(() => new RequestModel(Guid.Empty, null), maxSize: _appSettings.PoolSize);
            lifetime.ApplicationStopping.Register(() =>
            {
                _channel.Writer.Complete();
            });

            Task.Run(async () => await ExecuteAsync());
        }


        public async Task ProcessRequestAsync(HttpContext httpContext)
        {
            var tcs = new TaskCompletionSource<HttpResponseMessage>();
            var ct = new CancellationTokenSource(_requestTimeout);

            Guid guid = Guid.NewGuid();
            _targetRequstQueue.TryAdd(guid, tcs);
            ct.Token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);

            var requestModel = _requestModelPool.Rent();
            requestModel.Guid = guid;
            requestModel.Context = httpContext;
            await _channel.Writer.WriteAsync(requestModel);

            using var res = await tcs.Task;

            await CopyProxyHttpResponse(httpContext, res);
        }


        private async ValueTask ExecuteAsync()
        {
            await foreach (RequestModel item in _channel.Reader.ReadAllAsync())
            {
                try
                {
                    string url = GetTargetUri(item.Context, _appSettings.TargetServer);
                    using var requestMessage = CreateProxyHttpRequest(item.Context, url);
                    var responseMessage = await _httpClient.SendAsync(requestMessage);

                    if (_targetRequstQueue.TryRemove(item.Guid, out var tcs))
                    {
                        tcs.TrySetResult(responseMessage);
                    }
                }
                finally
                {
                    _requestModelPool.Return(item);
                }
               
            }
        }


        private static async Task CopyProxyHttpResponse(HttpContext context, HttpResponseMessage responseMessage)
        {
            context.Response.ContentType = responseMessage.Content.Headers.ContentType?.ToString();
            context.Response.ContentLength = responseMessage.Content.Headers.ContentLength;

            await responseMessage.Content.CopyToAsync(context.Response.Body);
        }

        private static string GetTargetUri(HttpContext context, string targetServer)
        {
            string http = "http";

            if (context.Request.IsHttps)
            {
                http = "https";
            }

            string url = $"{http}://{targetServer}{context.Request.Path}";
            if (context.Request.Query != null && context.Request.Query.Count > 0)
            {
                List<string> paras = new List<string>();
                foreach (var item in context.Request.Query)
                {
                    if (item.Value.Count > 0)
                    {
                        paras.Add($"{item.Key}={item.Value[0]}");
                    }

                }
                url = $"{url}?{string.Join("&", paras)}";
            }

            return url;
        }

        private static HttpRequestMessage CreateProxyHttpRequest(HttpContext context, string targetUri)
        {

            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(targetUri),
                Method = new HttpMethod(context.Request.Method)
            };

            // Body
            if (context.Request.ContentLength > 0 || context.Request.Headers.ContainsKey("Transfer-Encoding"))
            {
                requestMessage.Content = new StreamContent(context.Request.Body);

            }


            // Headers
            foreach (var header in context.Request.Headers)
            {
                if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                {
                    requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            return requestMessage;
        }

        private static BoundedChannelOptions GetChannelOptions(int batchPoolSize)
        {
            var channelOptions = new BoundedChannelOptions(batchPoolSize)
            {
                SingleWriter = false,
                SingleReader = true,
                AllowSynchronousContinuations = false,
                FullMode = BoundedChannelFullMode.Wait
            };

            return channelOptions;
        }
    }
}
