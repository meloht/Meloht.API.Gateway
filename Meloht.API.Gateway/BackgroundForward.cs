using Meloht.API.Gateway.LoadBalancing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace Meloht.API.Gateway
{
    public class BackgroundForward : BackgroundService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GatewayProxyHandler> _logger;
        private readonly IGatewayProxy _gatewayProxy;
        private readonly ILoadBalancingPolicy _loadBalancingPolicy;
        private readonly IServerProvider _serverProvider;

        public BackgroundForward(IHttpClientFactory httpClientFactory, ILoadBalancingPolicy loadBalancingPolicy, IServerProvider serverProvider, ILogger<GatewayProxyHandler> logger, IGatewayProxy gatewayProxy)
        {
            _logger = logger;
            _loadBalancingPolicy = loadBalancingPolicy;
            _serverProvider = serverProvider;
            _gatewayProxy = gatewayProxy;
            _httpClient = httpClientFactory.CreateClient(AppSettings.GatewayClient);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() =>
            {
                _gatewayProxy.RequestChannel.Writer.TryComplete();
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
            var servers = _serverProvider.GetServers();
            if (servers == null || servers.Count == 0)
            {
                throw new Exception("No target servers available.");
            }
            var targetServer = _loadBalancingPolicy.PickDestination(servers);
            if (targetServer == null)
            {
                throw new Exception("Failed to select target server.");
            }
            return targetServer.Address;
        }
        private async Task ForwardRequestAsync(RequestModel item)
        {
            try
            {
                string url = GetTargetUri(item.Context, GetTargetServer());
                using var requestMessage = CreateProxyHttpRequest(item.Context, url);
                var responseMessage = await _httpClient.SendAsync(requestMessage);

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
        private async Task ExecuteRequestAsync(CancellationToken stoppingToken)
        {
            await foreach (RequestModel item in _gatewayProxy.RequestChannel.Reader.ReadAllAsync(stoppingToken))
            {
                await ForwardRequestAsync(item);
            }
        }
    }
}
