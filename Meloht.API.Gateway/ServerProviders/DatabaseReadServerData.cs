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
    public abstract class DatabaseReadServerData
    {
        private readonly ILogger<DatabaseReadServerData> _logger;
        private readonly string _connectionString;

        protected abstract DbConnection GetDbConnection(string connectionString);
        protected abstract DbCommand GetDbCommand(string sql, DbConnection connection);

        private readonly Dictionary<string, ServerNode> _serversDict;
        private readonly List<ServerNode> _serversOriginalList;
        private readonly List<ServerNode> _serversHealthList;
        private readonly HealthCheckServer? _healthCheckServer;


        private readonly object _lock = new();
        private const int _databaseTimeoutSeconds = 2;

        public DatabaseReadServerData(IConfiguration config, ILogger<DatabaseReadServerData> logger, IServiceProvider provider)
        {
            _logger = logger;
            _connectionString = AppSettings.GetConnectionString(config);
            _serversDict = new Dictionary<string, ServerNode>();
            _serversOriginalList = new List<ServerNode>();
            _serversHealthList = new List<ServerNode>();
            _healthCheckServer = provider.GetService<HealthCheckServer>();
        }

        public async Task DataReadAsync(CancellationToken cancellationToken, ParallelOptions parallelOptions)
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
                        Weight = weight
                    });
                }
                List<ServerNode> serverNodes = AppUtils.UpdateData(servers, _serversDict);
                lock (_lock)
                {
                    _serversOriginalList.Clear();
                    _serversOriginalList.AddRange(serverNodes);
                }

                if (_healthCheckServer != null)
                {
                    await _healthCheckServer.CheckServerHealthAsync(parallelOptions, serverNodes);
                }

                lock (_lock)
                {
                    _serversHealthList.Clear();
                    _serversHealthList.AddRange(serverNodes.Where(p => p.Health == ServerHealth.Healthy));
                }

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
    }
}
