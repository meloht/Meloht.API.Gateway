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
    public class DatabaseAutoUpdateService : BackgroundService
    {
        private readonly ILogger<DatabaseAutoUpdateService> _logger;
        private readonly DatabaseReadServerData _serverProvider;
        private readonly ParallelOptions _parallelOptions;

        private int _autoUpdateIntervalSeconds;
        private readonly IOptionsMonitor<DatabaseAutoUpdateConfig> _options;

        public DatabaseAutoUpdateService(ILogger<DatabaseAutoUpdateService> logger, DatabaseReadServerData serverProvider, IOptionsMonitor<DatabaseAutoUpdateConfig> options)
        {
            _logger = logger;
            _options = options;
            _options.OnChange(OnConfigChanged);
            OnConfigChanged(options.CurrentValue);
            _serverProvider = serverProvider;
            _parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };

        }
        private void OnConfigChanged(DatabaseAutoUpdateConfig options)
        {
            if (options != null && options.IntervalSeconds > 0)
            {
                _autoUpdateIntervalSeconds = options.IntervalSeconds;
            }
            else
            {
                _autoUpdateIntervalSeconds = AppSettings.DatabaseIntervalSeconds;
            }
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _parallelOptions.CancellationToken = stoppingToken;
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("DatabaseAutoUpdate running at: {time}", DateTimeOffset.Now);
                await _serverProvider.DataReadAsync(_parallelOptions, stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(_autoUpdateIntervalSeconds), stoppingToken);
            }
        }
    }
}
