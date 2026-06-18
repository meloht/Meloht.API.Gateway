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
    public class ServerDataSourceMySql : DatabaseReadServerData
    {
        public ServerDataSourceMySql(ILogger<DatabaseReadServerData> logger, HealthCheckServer checkServer, IOptionsMonitor<DatabaseAutoUpdateConfig> options) : base(logger, checkServer, options)
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
