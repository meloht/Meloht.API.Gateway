using Meloht.API.Gateway.LoadBalancing;
using Meloht.API.Gateway.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Meloht.API.Gateway.ServerProviders
{
    public abstract class DatabaseReadServerData : ServerBase
    {
        private readonly ILogger<DatabaseReadServerData> _logger;
        private readonly string _connectionString;

        protected abstract DbConnection GetDbConnection(string connectionString);
        protected abstract DbCommand GetDbCommand(string sql, DbConnection connection);

        private readonly int _databaseTimeoutSeconds;

        public DatabaseReadServerData(IConfiguration config, ILogger<DatabaseReadServerData> logger, HealthCheckServer healthCheck) : base(healthCheck)
        {
            _logger = logger;
            _connectionString = AppSettings.GetConnectionString(config);
            _databaseTimeoutSeconds = AppSettings.GetDatabaseTimeoutSeconds(config);
        }


        public async Task DataReadAsync(ParallelOptions parallelOptions, CancellationToken cancellationToken)
        {
            try
            {
                using var conn = GetDbConnection(_connectionString);
                await conn.OpenAsync();

                string sql = "SELECT Id, UniqueName, Host, Port, Weight FROM server_nodes where active = 1 order by Id";

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
                    int port = reader.GetInt32(3);
                    int weight = reader.GetInt32(4);

                    servers.Add(new ServerNodeConfig
                    {
                        Id = id,
                        UniqueName = name,
                        Host = host,
                        Port = port,
                        Address = AppUtils.GetAddress(host, port),
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
