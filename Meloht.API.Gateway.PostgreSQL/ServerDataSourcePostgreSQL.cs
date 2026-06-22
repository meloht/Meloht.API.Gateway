using Meloht.API.Gateway.Common.Configuration;
using Meloht.API.Gateway.Common.Database;
using Meloht.API.Gateway.Common.HealthCheck;
using Meloht.API.Gateway.Configuration;
using Meloht.API.Gateway.ServerProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Meloht.API.Gateway.PostgreSQL
{
    public class ServerDataSourcePostgreSQL: DatabaseServerClient
    {
        public ServerDataSourcePostgreSQL(ILogger<DatabaseServerClient> logger, HealthCheckServer checkServer, IOptionsMonitor<DatabaseConfig> options) : base(logger, checkServer, options)
        {
          
           
        }
        protected override DbConnection GetDbConnection(string connectionString)
        {
            return new Npgsql.NpgsqlConnection(connectionString);
        }
        protected override DbCommand GetDbCommand(string sql, DbConnection connection)
        {
            return new Npgsql.NpgsqlCommand(sql, (Npgsql.NpgsqlConnection)connection);
        }
    }
}
