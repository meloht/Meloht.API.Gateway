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

        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<HttpResponseMessage>> _targetRequstQueue;
        private readonly ObjectPool<RequestModel> _requestModelPool;

        private readonly ILogger<GatewayProxyHandler> _logger;

        Channel<RequestModel> IGatewayProxy.RequestChannel => _channel;

        ConcurrentDictionary<Guid, TaskCompletionSource<HttpResponseMessage>> IGatewayProxy.TargetRequestQueue => _targetRequstQueue;

        public GatewayProxyHandler(IConfiguration configuration, IHostApplicationLifetime lifetime, ILogger<GatewayProxyHandler> logger)
        {
            _targetRequstQueue = new ConcurrentDictionary<Guid, TaskCompletionSource<HttpResponseMessage>>();

            _requestTimeout = configuration.GetValue<int>(AppSettings.HttpRequestTimeout) * 1000;
            _poolSize = configuration.GetValue<int>(AppSettings.PoolSize);

            _logger = logger;

            _channel = Channel.CreateBounded<RequestModel>(GetChannelOptions(_poolSize));
          
            _requestModelPool = new ObjectPool<RequestModel>(() => new RequestModel(Guid.Empty, null), maxSize: _poolSize);
            lifetime.ApplicationStopping.Register(() =>
            {
                _channel.Writer.TryComplete();
            });
        }


        public async Task ProcessRequestAsync(HttpContext httpContext)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request");
                throw;
            }
           
        }


        private static async Task CopyProxyHttpResponse(HttpContext context, HttpResponseMessage responseMessage)
        {
            context.Response.ContentType = responseMessage.Content.Headers.ContentType?.ToString();
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
