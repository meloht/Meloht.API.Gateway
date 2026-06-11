using Meloht.API.Gateway.ServerProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Meloht.API.Gateway.MySql
{
    public class ServerDataSourceMySql : DatabaseAutoUpdate
    {
        public ServerDataSourceMySql(IConfiguration config, ILogger<ServerProviderDatabase> logger) : base(config, logger)
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
