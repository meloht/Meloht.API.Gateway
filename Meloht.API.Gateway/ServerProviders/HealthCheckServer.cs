using Meloht.API.Gateway.Common;
using Meloht.API.Gateway.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private int _healthCheckTimeoutSeconds;
        private readonly IOptionsMonitor<HealthCheckConfig> _options;

        public HealthCheckServer(IHttpClientFactory httpClientFactory, ILogger<HealthCheckServer> logger, IOptionsMonitor<HealthCheckConfig> options)
        {
            _healthEndpoint = HealthCheckAPI.HealthCheckPath;
            _options = options;
            _options.OnChange(OnConfigChanged);
            OnConfigChanged(_options.CurrentValue);
            _httpClient = httpClientFactory.CreateClient(AppSettings.GatewayClient);
            _logger = logger;

        }
        private void OnConfigChanged(HealthCheckConfig options)
        {
            if (options != null && options.RequestTimeoutSeconds > 0)
            {
                _healthCheckTimeoutSeconds = options.RequestTimeoutSeconds;
            }
            else
            {
                _healthCheckTimeoutSeconds = AppSettings.HealthChecTimeoutSeconds;
            }
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
                var response = await _httpClient.GetAsync($"http://{server.Address}{_healthEndpoint}", cancellationToken);
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
