using Meloht.API.Gateway.LoadBalancing;
using Meloht.API.Gateway.ServerProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.HostServices
{
    internal class ServerHealthCheckService : BackgroundService
    {
        private readonly ILogger<ServerHealthCheckService> _logger;
        private readonly IServerProvider _serverProvider;
        private readonly HealthCheckServer _healthCheckServer;

        private readonly ParallelOptions _parallelOptions;
        private const int _healthCheckIntervalSeconds = 30;
        public ServerHealthCheckService(IServerProvider serverProvider, HealthCheckServer healthCheckServer, ILogger<ServerHealthCheckService> logger)
        {
            _serverProvider = serverProvider;
            _healthCheckServer = healthCheckServer;
            _logger = logger;
            _parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _parallelOptions.CancellationToken = stoppingToken;
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("HealthCheckServer running at: {time}", DateTimeOffset.Now);
                await CheckHealthAsync(_parallelOptions);
                await Task.Delay(TimeSpan.FromSeconds(_healthCheckIntervalSeconds), stoppingToken);
            }
        }

        private async Task CheckHealthAsync(ParallelOptions parallelOptions)
        {
            var servers = _serverProvider.GetOriginalServers();

            await _healthCheckServer.CheckServerHealthAsync(_parallelOptions, servers);
        }
    }
}
