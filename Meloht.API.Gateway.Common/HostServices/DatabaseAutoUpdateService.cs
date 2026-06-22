using Meloht.API.Gateway.Common.Configuration;
using Meloht.API.Gateway.Common.Database;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Common.HostServices
{
    public class DatabaseAutoUpdateService : BackgroundService
    {
        private readonly ILogger<DatabaseAutoUpdateService> _logger;
        private readonly DatabaseServerClient _serverProvider;
        private readonly ParallelOptions _parallelOptions;

        private int _autoUpdateIntervalSeconds;
        private readonly IOptionsMonitor<DatabaseConfig> _options;

        public DatabaseAutoUpdateService(ILogger<DatabaseAutoUpdateService> logger, DatabaseServerClient serverProvider, IOptionsMonitor<DatabaseConfig> options)
        {
            _logger = logger;
            _options = options;
            _options.OnChange(OnConfigChanged);
            OnConfigChanged(options.CurrentValue);
            _serverProvider = serverProvider;
            _parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };

        }
        private void OnConfigChanged(DatabaseConfig options)
        {
            if (options != null && options.IntervalSeconds > 0)
            {
                _autoUpdateIntervalSeconds = options.IntervalSeconds;
            }
            else
            {
                _autoUpdateIntervalSeconds = AppSettingsCore.DatabaseIntervalSeconds;
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
