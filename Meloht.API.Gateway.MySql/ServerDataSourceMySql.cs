using Meloht.API.Gateway.Common.Configuration;
using Meloht.API.Gateway.Common.Database;
using Meloht.API.Gateway.Common.HealthCheck;
using Meloht.API.Gateway.Configuration;
using Meloht.API.Gateway.ServerProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Meloht.API.Gateway.MySql
{
    public class ServerDataSourceMySql : DatabaseServerClient
    {
        public ServerDataSourceMySql(ILogger<DatabaseServerClient> logger, HealthCheckServer checkServer, IOptionsMonitor<DatabaseConfig> options) : base(logger, checkServer, options)
        {
        }

        protected override DbCommand GetDbCommand(string sql, DbConnection connection)
        {
            return new MySqlCommand(sql, (MySqlConnection)connection);
        }

        protected override DbConnection GetDbConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }
    }
}
