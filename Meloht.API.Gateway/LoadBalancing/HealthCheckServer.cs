using Meloht.API.Gateway.ServerProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Meloht.API.Gateway.LoadBalancing
{
    public class HealthCheckServer : BackgroundService
    {
        private readonly ILogger<HealthCheckServer> _logger;
        private readonly IServerProvider _serverProvider;
        private readonly HttpClient _httpClient;
        private const string _testEndpoint = "/health";
        private readonly ParallelOptions _parallelOptions;
        private const int _healthCheckIntervalSeconds = 30;
        private const int _healthCheckTimeoutSeconds = 2;

        public HealthCheckServer(IServerProvider serverProvider, IHttpClientFactory httpClientFactory, ILogger<HealthCheckServer> logger)
        {
            _serverProvider = serverProvider;
            _httpClient = httpClientFactory.CreateClient(AppSettings.GatewayClient);
            _logger = logger;
            _parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _parallelOptions.CancellationToken = stoppingToken;
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await CheckHealthAsync(_parallelOptions);
                await Task.Delay(TimeSpan.FromSeconds(_healthCheckIntervalSeconds), stoppingToken);
            }
        }

        private async Task CheckHealthAsync(ParallelOptions parallelOptions)
        {
            var servers = _serverProvider.GetServers();

            await CheckServerHealthAsync(_parallelOptions, servers);
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
