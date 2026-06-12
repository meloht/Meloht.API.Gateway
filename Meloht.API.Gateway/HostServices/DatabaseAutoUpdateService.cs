using Meloht.API.Gateway.ServerProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

        private readonly int _autoUpdateIntervalSeconds;

        public DatabaseAutoUpdateService(ILogger<DatabaseAutoUpdateService> logger, DatabaseReadServerData serverProvider, IConfiguration config)
        {
            _logger = logger;
            _serverProvider = serverProvider;
            _parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
            _autoUpdateIntervalSeconds = AppSettings.GetDatabaseAutoUpdateIntervalSeconds(config);
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
