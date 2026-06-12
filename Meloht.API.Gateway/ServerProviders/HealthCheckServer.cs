using Meloht.API.Gateway.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Meloht.API.Gateway.ServerProviders
{
    public class HealthCheckServer
    {
        private readonly ILogger<HealthCheckServer> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _healthEndpoint;
        private readonly int _healthCheckTimeoutSeconds;

        public HealthCheckServer(IHttpClientFactory httpClientFactory, ILogger<HealthCheckServer> logger, IConfiguration configuration)
        {
            _healthEndpoint = HealthCheckAPI.HealthCheckPath;
            _httpClient = httpClientFactory.CreateClient(AppSettings.GatewayClient);
            _logger = logger;
            _healthCheckTimeoutSeconds = AppSettings.GetHealthRequestTimeoutSeconds(configuration);
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
                var response = await _httpClient.GetAsync($"{server.Address}{_healthEndpoint}", cancellationToken);
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
