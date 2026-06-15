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
    public class ServerDataSourcePostgreSQL: DatabaseReadServerData
    {
        public ServerDataSourcePostgreSQL(ILogger<DatabaseReadServerData> logger, HealthCheckServer checkServer, IOptionsMonitor<DatabaseAutoUpdateConfig> options) : base(logger, checkServer, options)
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
