using Meloht.API.Gateway.Configuration;
using Meloht.API.Gateway.ServerProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private int _healthCheckIntervalSeconds;
        private readonly IOptionsMonitor<HealthCheckConfig> _options;
        public ServerHealthCheckService(IServerProvider serverProvider, HealthCheckServer healthCheckServer, ILogger<ServerHealthCheckService> logger, IOptionsMonitor<HealthCheckConfig> options)
        {
            _serverProvider = serverProvider;
            _healthCheckServer = healthCheckServer;
            _options = options;
            _options.OnChange(OnConfigChanged);
            OnConfigChanged(_options.CurrentValue);
            _logger = logger;
          
            _parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
        }
        private void OnConfigChanged(HealthCheckConfig options)
        {
            if (options != null && options.IntervalSeconds > 0)
            {
                _healthCheckIntervalSeconds = options.IntervalSeconds;
            }
            else
            {
                _healthCheckIntervalSeconds = AppSettings.HealthCheckIntervalSeconds;
            }
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

            _serverProvider.UpdateHealthListByHealthService();
        }
    }
}
