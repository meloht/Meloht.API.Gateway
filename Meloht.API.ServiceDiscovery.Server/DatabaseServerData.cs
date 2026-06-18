using Meloht.API.Gateway.Common.Configuration;
using Meloht.API.ServiceDiscovery.Server.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Meloht.API.ServiceDiscovery.Server
{
    public abstract class DatabaseServerData
    {
        private readonly ILogger<DatabaseServerData> _logger;

        protected abstract DbConnection GetDbConnection(string connectionString);
        protected abstract DbCommand GetDbCommand(string sql, DbConnection connection);

        protected abstract void AddParameterString(DbCommand cmd, string name, string val);
        protected abstract void AddParameterInt(DbCommand cmd, string name, int val);


        private DatabaseConfig _databaseConfig;

        public DatabaseServerData(ILogger<DatabaseServerData> logger, IOptionsMonitor<DatabaseConfig> options)
        {
            _logger = logger;
            _databaseConfig = options.CurrentValue;
            options.OnChange(val => _databaseConfig = val);
        }


        public async Task<bool> RegisterSaveAndUpdateAsync(ServerNodeConfig serverConfig)
        {
            ValidationData(serverConfig);
            using var conn = GetDbConnection(_databaseConfig.ConnectionString);
            await conn.OpenAsync();

            string sql = "SELECT UniqueName FROM server_nodes where UniqueName=@name";

            using var cmd = GetDbCommand(sql, conn);
            AddParameterString(cmd, "name", serverConfig.UniqueName);
            cmd.CommandTimeout = _databaseConfig.DatabaseTimeoutSeconds;
            using var ct = new CancellationTokenSource(TimeSpan.FromSeconds(_databaseConfig.DatabaseTimeoutSeconds));
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
            using var conn = GetDbConnection(_databaseConfig.ConnectionString);
            await conn.OpenAsync();

            string sql = "SELECT UniqueName FROM server_nodes where UniqueName=@name";

            using var cmd = GetDbCommand(sql, conn);
            AddParameterString(cmd, "name", serverConfig.UniqueName);
            cmd.CommandTimeout = _databaseConfig.DatabaseTimeoutSeconds;
            using var ct = new CancellationTokenSource(TimeSpan.FromSeconds(_databaseConfig.DatabaseTimeoutSeconds));
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
