using Meloht.API.Gateway.Common.Configuration;
using Meloht.API.Gateway.Common.HealthCheck;
using Meloht.API.Gateway.Common.Utilities;
using Meloht.API.Gateway.ServerProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Meloht.API.Gateway.Common.Database
{
    public abstract class DatabaseServerClient: ServerBase
    {
        private readonly ILogger<DatabaseServerClient> _logger;
        protected string _connectionString;

        protected abstract DbConnection GetDbConnection(string connectionString);
        protected abstract DbCommand GetDbCommand(string sql, DbConnection connection);

        protected int _databaseTimeoutSeconds;

        private readonly IOptionsMonitor<DatabaseConfig> _options;

        public DatabaseServerClient(ILogger<DatabaseServerClient> logger, HealthCheckServer healthCheck, IOptionsMonitor<DatabaseConfig> options) : base(healthCheck)
        {
            _logger = logger;
            _options = options;
            _options.OnChange(OnConfigChanged);
            OnConfigChanged(_options.CurrentValue);
        }

        private void OnConfigChanged(DatabaseConfig options)
        {
            if (options != null && options.DatabaseTimeoutSeconds > 0)
            {
                _databaseTimeoutSeconds = options.DatabaseTimeoutSeconds;
            }
            else
            {
                _databaseTimeoutSeconds = AppSettingsCore.DatabaseExecuteTimeoutSeconds;
            }

            if (options != null && !string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                _connectionString = options.ConnectionString;
            }
            else
            {
                throw new ArgumentNullException("ConnectionString");
            }

        }
        public async Task DataReadAsync(ParallelOptions parallelOptions, CancellationToken cancellationToken)
        {
            try
            {
                using var conn = GetDbConnection(_connectionString);
                await conn.OpenAsync();

                string sql = "SELECT Id, UniqueName, Host, Protocol, Weight FROM server_nodes where active = 1 order by Id";

                using var cmd = GetDbCommand(sql, conn);

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(_databaseTimeoutSeconds));

                using var reader = await cmd.ExecuteReaderAsync(cts.Token);

                List<ServerNodeConfig> servers = new List<ServerNodeConfig>();
                while (await reader.ReadAsync())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    string host = reader.GetString(2);
                    string protocol = reader.GetString(3);
                    int weight = reader.GetInt32(4);

                    servers.Add(new ServerNodeConfig
                    {
                        Id = id,
                        UniqueName = name,
                        Host = host,
                        Protocol = protocol,
                        Weight = AppUtils.GetWeight(weight),
                    });
                }
                await conn.CloseAsync();
                List<ServerNode> serverNodes = AppUtils.UpdateData(servers, _serversDict);

                UpdateOriginalList(serverNodes);
                await _healthCheckServer.CheckServerHealthAsync(parallelOptions, serverNodes);
                UpdateHealthlList(serverNodes);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading server nodes from database");
            }

        }

        public List<ServerNode> GetHealthyServers()
        {
            return _serversHealthList;
        }

        public List<ServerNode> GetAllServers()
        {
            return _serversOriginalList;
        }

        public int GetServerWeightSum()
        {
            return _weightSum;
        }
    }
}
