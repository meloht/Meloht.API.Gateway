using Meloht.API.Gateway.ServerProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Meloht.API.Gateway.PostgreSQL
{
    public class ServerDataSourcePostgreSQL: ServerProviderDatabase
    {
        public ServerDataSourcePostgreSQL(IConfiguration config, ILogger<ServerProviderDatabase> logger) : base(config, logger)
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
