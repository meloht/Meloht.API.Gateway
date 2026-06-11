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

        private const int _databaseTimeoutSeconds = 2;

        public DatabaseReadServerData(IConfiguration config, ILogger<DatabaseReadServerData> logger, IServiceProvider provider) : base(provider)
        {
            _logger = logger;
            _connectionString = AppSettings.GetConnectionString(config);
        }


        public async Task DataReadAsync(ParallelOptions parallelOptions, CancellationToken cancellationToken)
        {
            try
            {
                using var conn = GetDbConnection(_connectionString);
                await conn.OpenAsync();

                string sql = "SELECT Id, Name, Address, Weight FROM server_nodes where active = 1 order by Id";

                using var cmd = GetDbCommand(sql, conn);

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(_databaseTimeoutSeconds));

                using var reader = await cmd.ExecuteReaderAsync(cts.Token);

                List<ServerNodeConfig> servers = new List<ServerNodeConfig>();
                while (await reader.ReadAsync())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    string address = reader.GetString(2);
                    int weight = reader.GetInt32(3);

                    servers.Add(new ServerNodeConfig
                    {
                        Id = id,
                        UniqueName = name,
                        Address = address,
                        Weight = GetWeight(weight),
                    });
                }
                await conn.CloseAsync();
                List<ServerNode> serverNodes = AppUtils.UpdateData(servers, _serversDict);

                UpdateOriginalList(serverNodes);
                if (_healthCheckServer != null)
                {
                    await _healthCheckServer.CheckServerHealthAsync(parallelOptions, serverNodes);
                }
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
