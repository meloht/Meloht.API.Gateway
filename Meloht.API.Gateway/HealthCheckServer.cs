using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Meloht.API.Gateway
{
    public class HealthCheckServer : BackgroundService
    {
        private readonly ILogger<HealthCheckServer> _logger;
        private readonly IServerProvider _serverProvider;
        private readonly HttpClient _httpClient;
        private const string _testEndpoint = "/health";
        private readonly ParallelOptions _parallelOptions;
        private const int _healthCheckIntervalSeconds = 30;
        public HealthCheckServer(IServerProvider serverProvider, IHttpClientFactory httpClientFactory, ILogger<HealthCheckServer> logger)
        {
            _serverProvider = serverProvider;
            _httpClient = httpClientFactory.CreateClient(ServiceCollectionExtensions.GatewayClient);
            _logger = logger;
            _parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 100 };

        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await CheckHealthAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(_healthCheckIntervalSeconds), stoppingToken);
            }
        }

        public async Task CheckHealthAsync(CancellationToken token)
        {
            var servers = _serverProvider.GetServers();

            await Parallel.ForEachAsync(servers, _parallelOptions, async (server, ct) =>
            {
                await CheckServerHealth(server, ct);
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
