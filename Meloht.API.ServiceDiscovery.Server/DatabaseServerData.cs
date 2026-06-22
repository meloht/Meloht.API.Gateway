using Meloht.API.Gateway.Common.Configuration;
using Meloht.API.Gateway.Common.Database;
using Meloht.API.Gateway.Common.HealthCheck;
using Meloht.API.Gateway.Common.Utilities;
using Meloht.API.ServiceDiscovery.Server.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Meloht.API.ServiceDiscovery.Server
{
    public abstract class DatabaseServerData: DatabaseServerClient
    {
        protected abstract void AddParameterString(DbCommand cmd, string name, string val);
        protected abstract void AddParameterInt(DbCommand cmd, string name, int val);

        public DatabaseServerData(ILogger<DatabaseServerClient> loggerClient, HealthCheckServer healthCheck, IOptionsMonitor<DatabaseConfig> options)
            :base(loggerClient, healthCheck, options)
        {
        }


        public async Task<bool> RegisterSaveAndUpdateAsync(ServerNodeConfig serverConfig)
        {
            ValidationData(serverConfig);
            using var conn = GetDbConnection(_connectionString);
            await conn.OpenAsync();

            string sql = "SELECT UniqueName FROM server_nodes where UniqueName=@name";

            using var cmd = GetDbCommand(sql, conn);
            AddParameterString(cmd, "name", serverConfig.UniqueName);
            cmd.CommandTimeout = _databaseTimeoutSeconds;
            using var ct = new CancellationTokenSource(TimeSpan.FromSeconds(_databaseTimeoutSeconds));
            var res = await cmd.ExecuteScalarAsync(ct.Token);
            string sqlData;
            if (res != null && res.ToString().Trim() == serverConfig.UniqueName)
            {
                sqlData = "update server_nodes set Host=@Host,Protocol=@Protocol,Weight=@Weight where UniqueName=@UniqueName";
            }
            else
            {
                sqlData = "insert into server_nodes(UniqueName, Host, Protocol, Weight) values(@UniqueName,@Host,@Protocol,@Weight)";
            }
            cmd.Parameters.Clear();
            AddParameterString(cmd, "UniqueName", serverConfig.UniqueName);
            AddParameterString(cmd, "Host", serverConfig.Host);
            AddParameterString(cmd, "Protocol", serverConfig.Protocol);
            AddParameterInt(cmd, "Weight", serverConfig.Weight);
            cmd.CommandText = sqlData;
            int rent = await cmd.ExecuteNonQueryAsync();
            return rent > 0;
        }

        public async Task<bool> UnregisterUpdateAsync(ServerNodeConfig serverConfig)
        {
            ValidationData(serverConfig);
            using var conn = GetDbConnection(_connectionString);
            await conn.OpenAsync();

            string sql = "SELECT UniqueName FROM server_nodes where UniqueName=@name";

            using var cmd = GetDbCommand(sql, conn);
            AddParameterString(cmd, "name", serverConfig.UniqueName);
            cmd.CommandTimeout = _databaseTimeoutSeconds;
            using var ct = new CancellationTokenSource(TimeSpan.FromSeconds(_databaseTimeoutSeconds));
            var res = await cmd.ExecuteScalarAsync(ct.Token);

            if (res != null && res.ToString().Trim() == serverConfig.UniqueName)
            {
                string sqlData = "update server_nodes set active=0 where UniqueName=@UniqueName";
                cmd.Parameters.Clear();
                AddParameterString(cmd, "UniqueName", serverConfig.UniqueName);
                cmd.CommandText = sqlData;
                int rent = await cmd.ExecuteNonQueryAsync();
                return rent > 0;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<ServerNodeConfig>> GetClientsFromDatabaseAsync(CancellationToken cancellationToken)
        {
            using var conn = GetDbConnection(_connectionString);
            await conn.OpenAsync();
            string sql = "SELECT Id, UniqueName, Host, Protocol, Weight FROM server_nodes where active = 1 order by Id";
            using var cmd = GetDbCommand(sql, conn);
            cmd.CommandTimeout = _databaseTimeoutSeconds;
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

            return servers;
        }

        private void ValidationData(ServerNodeConfig serverConfig)
        {
            
            if (string.IsNullOrWhiteSpace(serverConfig.UniqueName))
            {
                throw new ArgumentException("serverConfig.UniqueName is null or empty");
            }
            if (string.IsNullOrWhiteSpace(serverConfig.Protocol))
            {
                throw new ArgumentException("serverConfig.Protocol is null or empty");
            }
            if (string.IsNullOrWhiteSpace(serverConfig.Host))
            {
                throw new ArgumentException("serverConfig.Host is null or empty");
            }
            if (serverConfig.Weight < 1 || serverConfig.Weight > 100)
            {
                throw new ArgumentException("serverConfig.Weight must be between 1 and 100");
            }
        }
    }
}
