using Meloht.API.Gateway.Common;
using Meloht.API.Gateway.Common.Configuration;
using Meloht.API.Gateway.Common.Database;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.ServiceDiscovery
{
    public class ServiceDiscoveryHost : BackgroundService
    {

        private readonly ILogger<ServiceDiscoveryHost> _logger;
        private int _intervalSeconds;
        private readonly ParallelOptions _parallelOptions;
        private readonly ServiceDiscoveryClient _discoveryClient;

        public ServiceDiscoveryHost(ILogger<ServiceDiscoveryHost> logger, IOptionsMonitor<ServiceDiscoveryConfig> options, ServiceDiscoveryClient discoveryClient)
        {
            _logger = logger;
            _discoveryClient = discoveryClient;
            _parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
            options.OnChange(OnConfigChange);
            OnConfigChange(options.CurrentValue);
        }
        private void OnConfigChange(ServiceDiscoveryConfig config)
        {
            if (config != null && config.IntervalSeconds > 0)
            {
                _intervalSeconds = config.IntervalSeconds;
            }
            else
            {
                _intervalSeconds = ServiceDiscoveryConfig.IntervalSecondsDefault;
            }


        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _parallelOptions.CancellationToken = stoppingToken;
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("ServiceDiscovery running at: {time}", DateTimeOffset.Now);
                await _discoveryClient.DataReadAsync(_parallelOptions, stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(_intervalSeconds), stoppingToken);
            }
        }
    }
}
