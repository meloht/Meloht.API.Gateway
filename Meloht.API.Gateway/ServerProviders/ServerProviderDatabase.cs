using Meloht.API.Gateway.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Meloht.API.Gateway.ServerProviders
{
    public abstract class ServerProviderDatabase : IServerProvider
    {

        protected abstract DbConnection GetDbConnection(string connectionString);
        protected abstract DbCommand GetDbCommand(string sql, DbConnection connection);

        private readonly ILogger<ServerProviderDatabase> _logger;

        private readonly string _connectionString;

        private Dictionary<string, ServerNode> _serversDict;
        private readonly List<ServerNode> _serversList;
        private readonly object _lock = new();
        public ServerProviderDatabase(IConfiguration config, ILogger<ServerProviderDatabase> logger)
        {
            _serversDict = new Dictionary<string, ServerNode>();
            _serversList = new List<ServerNode>();
            _connectionString = AppSettings.GetConnectionString(config);
            _logger = logger;
        }


        public IReadOnlyList<ServerNode> GetServers()
        {
            lock (_lock)
            {
                return _serversList;
            }
        }


        public async Task DataReadAsync()
        {
            try
            {
                using var conn = GetDbConnection(_connectionString);
                await conn.OpenAsync();

                string sql = "SELECT Id, Name, Address, Weight FROM server_nodes where active = 1 order by Id";

                using var cmd = GetDbCommand(sql, conn);

                using var reader = await cmd.ExecuteReaderAsync();

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

                lock (_lock)
                {
                    AppUtils.UpdateData(servers, _serversDict, _serversList);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading server nodes from database");
            }
           
        }
    }
}
