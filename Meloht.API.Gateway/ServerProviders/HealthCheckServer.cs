using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Meloht.API.Gateway.ServerProviders
{
    internal class HealthCheckServer 
    {
        private readonly ILogger<HealthCheckServer> _logger;
        private readonly HttpClient _httpClient;
        private const string _testEndpoint = "/health/live";
        private const int _healthCheckTimeoutSeconds = 5;

        public HealthCheckServer(IHttpClientFactory httpClientFactory, ILogger<HealthCheckServer> logger)
        {
            _httpClient = httpClientFactory.CreateClient(AppSettings.GatewayClient);
            _logger = logger;
        }


        public async Task CheckServerHealthAsync(ParallelOptions parallelOptions, IReadOnlyList<ServerNode> servers)
        {
            await Parallel.ForEachAsync(servers, parallelOptions, async (server, ct) =>
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                cts.CancelAfter(TimeSpan.FromSeconds(_healthCheckTimeoutSeconds));
                await CheckServerHealth(server, cts.Token);
            });
        }

        private async Task CheckServerHealth(ServerNode server, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{server.Address}{_testEndpoint}", cancellationToken);
                server.Health = response.IsSuccessStatusCode ? ServerHealth.Healthy : ServerHealth.Unhealthy;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking health of server {server}", server.Address);
                server.Health = ServerHealth.Unhealthy;
            }
        }
    }
}
