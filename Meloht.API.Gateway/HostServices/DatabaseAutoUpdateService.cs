using Meloht.API.Gateway.ServerProviders;
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

        private const int _autoUpdateIntervalSeconds = 120;

        public DatabaseAutoUpdateService(ILogger<DatabaseAutoUpdateService> logger, DatabaseReadServerData serverProvider)
        {
            _logger = logger;
            _serverProvider = serverProvider;
            _parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _parallelOptions.CancellationToken = stoppingToken;
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("DatabaseAutoUpdate running at: {time}", DateTimeOffset.Now);
                await _serverProvider.DataReadAsync(stoppingToken, _parallelOptions);
                await Task.Delay(TimeSpan.FromSeconds(_autoUpdateIntervalSeconds), stoppingToken);
            }
        }
    }
}
