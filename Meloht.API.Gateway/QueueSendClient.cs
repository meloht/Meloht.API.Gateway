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
    public class QueueSendClient: IGatewayProxy
    {

        private readonly Channel<RequestModel> _channel;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly AppSettingsClient _appSettings;
        private int _requestTimeout;

        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<HttpResponseMessage>> _targetRequstQueue;

        public QueueSendClient(IConfiguration configuration, IHttpClientFactory httpClientFactory, IHostApplicationLifetime lifetime)
        {
            _targetRequstQueue = new ConcurrentDictionary<Guid, TaskCompletionSource<HttpResponseMessage>>();
            _configuration = configuration;
            _appSettings = new AppSettingsClient(_configuration);
            _httpClient = httpClientFactory.CreateClient();
            _channel = Channel.CreateBounded<RequestModel>(GetChannelOptions(_appSettings.PoolSize));
            _requestTimeout = _appSettings.HttpRequestTimeout * 1000;
            lifetime.ApplicationStopping.Register(() =>
            {
                _channel.Writer.Complete();
            });

            Task.Run(async () => await ExecuteAsync(""));
        }


        public async Task ProcessRequestAsync(HttpContext httpContext)
        {
            var tcs = new TaskCompletionSource<HttpResponseMessage>();
            var ct = new CancellationTokenSource(_requestTimeout);

            Guid guid = Guid.NewGuid();
            _targetRequstQueue.TryAdd(guid, tcs);
            ct.Token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);

            await _channel.Writer.WriteAsync(new RequestModel(guid, httpContext));

            using var res = await tcs.Task;

            await CopyProxyHttpResponse(httpContext, res);
        }


        private async ValueTask ExecuteAsync(string targetUri)
        {
            await foreach (RequestModel item in _channel.Reader.ReadAllAsync())
            {
                using var requestMessage = CreateProxyHttpRequest(item.context, targetUri);
                var responseMessage = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, item.context.RequestAborted);

                if (_targetRequstQueue.TryRemove(item.Guid, out var tcs))
                {
                    tcs.TrySetResult(responseMessage);
                }
            }
        }


        private static async Task CopyProxyHttpResponse(HttpContext context, HttpResponseMessage responseMessage)
        {
            context.Response.StatusCode = (int)responseMessage.StatusCode;

            foreach (var header in responseMessage.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();

            }

            // Kestrel自己计算
            context.Response.Headers.Remove("transfer-encoding");
            await responseMessage.Content.CopyToAsync(context.Response.Body);
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
