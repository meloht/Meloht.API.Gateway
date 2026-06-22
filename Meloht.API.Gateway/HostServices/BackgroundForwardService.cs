using Meloht.API.Gateway.Common;
using Meloht.API.Gateway.Common.HealthCheck;
using Meloht.API.Gateway.Common.Utilities;
using Meloht.API.Gateway.LoadBalancing;
using Meloht.API.Gateway.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace Meloht.API.Gateway.HostServices
{
    internal class BackgroundForwardService : BackgroundService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GatewayProxyHandler> _logger;
        private readonly IGatewayProxy _gatewayProxy;
        private readonly ILoadBalancingPolicy _loadBalancingPolicy;
        private readonly IServerProvider _serverProvider;
        private readonly ObjectPool<StringBuilder> _poolStringBuilder;

        public BackgroundForwardService(IHttpClientFactory httpClientFactory, ILoadBalancingPolicy loadBalancingPolicy, IServerProvider serverProvider, ILogger<GatewayProxyHandler> logger, IGatewayProxy gatewayProxy)
        {
            _logger = logger;
            _loadBalancingPolicy = loadBalancingPolicy;
            _serverProvider = serverProvider;
            _gatewayProxy = gatewayProxy;
            _httpClient = httpClientFactory.CreateClient(HttpClientKey.ClientKey);
            _poolStringBuilder = new ObjectPool<StringBuilder>(() => new StringBuilder(), maxSize: AppUtils.GetObjectPoolSize(), resetAction: ResetStringBuilder);
        }
        private void ResetStringBuilder(StringBuilder sb)
        {
            sb.Clear();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() =>
            {
                _gatewayProxy.RequestChannel.Writer.TryComplete();
                _poolStringBuilder.Dispose();
            });
            Task[] tasks = new Task[Environment.ProcessorCount];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = ExecuteRequestAsync(stoppingToken);
            }
            await Task.WhenAll(tasks);
        }
        private string GetTargetServer()
        {
            var servers = _serverProvider.GetCluster();
            if (servers == null || servers.Servers.Length == 0)
            {
                throw new Exception("No target servers available.");
            }
            var targetServer = _loadBalancingPolicy.PickDestination(_serverProvider.GetCluster());
            if (targetServer == null)
            {
                throw new Exception("Failed to select target server.");
            }
            if (targetServer.Host == null)
            {
                throw new Exception("targetServer host is null");
            }
            return targetServer.Host;
        }
        private async Task ForwardRequestAsync(RequestModel item)
        {
            try
            {
                string url = GetTargetUri(item.Context, GetTargetServer());
                using var requestMessage = CreateProxyHttpRequest(item.Context, url);
                var responseMessage = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, item.Context.RequestAborted);

                if (_gatewayProxy.TargetRequestQueue.TryRemove(item.Guid, out var tcs))
                {
                    tcs.TrySetResult(responseMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding request");
            }
            finally
            {
                _gatewayProxy.ReturnPool(item);
            }
        }
        private string GetTargetUri(HttpContext context, string targetServer)
        {
            StringBuilder urlBuilder = _poolStringBuilder.Rent();
            try
            {
                urlBuilder.Append("http");

                if (context.Request.IsHttps)
                {
                    urlBuilder.Append('s');
                }

                urlBuilder.Append("://").Append(targetServer).Append(context.Request.Path);

                if (context.Request.Query != null && context.Request.Query.Count > 0)
                {
                    urlBuilder.Append(context.Request.QueryString.Value);
                }

                return urlBuilder.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetTargetUri Error: {ex.Message}");
                throw;
            }
            finally
            {
                _poolStringBuilder.Return(urlBuilder);
            }

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
        private async Task ExecuteRequestAsync(CancellationToken stoppingToken)
        {
            await foreach (RequestModel item in _gatewayProxy.RequestChannel.Reader.ReadAllAsync(stoppingToken))
            {
                await ForwardRequestAsync(item);
            }
        }
    }
}
