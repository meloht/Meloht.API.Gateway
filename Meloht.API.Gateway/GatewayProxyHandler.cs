using Meloht.API.Gateway.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace Meloht.API.Gateway
{
    public class GatewayProxyHandler : IGatewayProxy
    {

        private readonly Channel<RequestModel> _channel;
        private readonly int _requestTimeout;
        private readonly int _poolSize;

        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<HttpResponseMessage>> _targetRequestQueue;
        private readonly ObjectPool<RequestModel> _requestModelPool;

        private readonly ILogger<GatewayProxyHandler> _logger;

        Channel<RequestModel> IGatewayProxy.RequestChannel => _channel;

        ConcurrentDictionary<Guid, TaskCompletionSource<HttpResponseMessage>> IGatewayProxy.TargetRequestQueue => _targetRequestQueue;

        public GatewayProxyHandler(IConfiguration configuration, IHostApplicationLifetime lifetime, ILogger<GatewayProxyHandler> logger)
        {
            _targetRequestQueue = new ConcurrentDictionary<Guid, TaskCompletionSource<HttpResponseMessage>>();

            _requestTimeout = AppSettings.GetHttpRequestTimeout(configuration) * 1000;
            _poolSize = AppSettings.GetPoolSize(configuration);

            _logger = logger;

            _channel = Channel.CreateBounded<RequestModel>(GetChannelOptions(_poolSize));

            _requestModelPool = new ObjectPool<RequestModel>(() => new RequestModel(Guid.Empty, null), maxSize: AppSettings.GetObjectPoolSize(), resetAction: ResetRequestModel);
            lifetime.ApplicationStopping.Register(() =>
            {
                _channel.Writer.TryComplete();
            });
        }

        private void ResetRequestModel(RequestModel requestModel)
        {
            requestModel.Guid = Guid.Empty;
            requestModel.Context = null;
        }

        public async Task ProcessRequestAsync(HttpContext httpContext)
        {
            try
            {
                var tcs = new TaskCompletionSource<HttpResponseMessage>();
                
                using var ct = new CancellationTokenSource(_requestTimeout);

                Guid guid = Guid.NewGuid();
                _targetRequestQueue.TryAdd(guid, tcs);
                ct.Token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);

                var requestModel = _requestModelPool.Rent();
                requestModel.Guid = guid;
                requestModel.Context = httpContext;
                await _channel.Writer.WriteAsync(requestModel);

                using var res = await tcs.Task;

                await CopyProxyHttpResponse(httpContext, res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request");
                throw;
            }

        }


        private static async Task CopyProxyHttpResponse(HttpContext context, HttpResponseMessage responseMessage)
        {
            
            if (responseMessage.Content.Headers.ContentType != null)
            {
                context.Response.ContentType = responseMessage.Content.Headers.ContentType.ToString();
            }
            else
            {
                if (responseMessage.Content.Headers.TryGetValues("Content-Type", out var values))
                {
                    context.Response.ContentType = string.Join(";", values);
                }
            }

            context.Response.ContentLength = responseMessage.Content.Headers.ContentLength;

            await responseMessage.Content.CopyToAsync(context.Response.Body);
        }


        private static BoundedChannelOptions GetChannelOptions(int batchPoolSize)
        {
            var channelOptions = new BoundedChannelOptions(batchPoolSize)
            {
                SingleWriter = false,
                SingleReader = false,
                AllowSynchronousContinuations = false,
                FullMode = BoundedChannelFullMode.Wait
            };

            return channelOptions;
        }


        void IGatewayProxy.ReturnPool(RequestModel item)
        {
            _requestModelPool.Return(item);
        }
    }
}
