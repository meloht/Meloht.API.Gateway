using Meloht.API.Gateway.Common.Configuration;
using Meloht.API.Gateway.Common.HealthCheck;
using Meloht.API.ServiceDiscovery.Server;
using Meloht.API.ServiceDiscovery.Server.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Meloht.API.ServiceDiscovery.Provider.PostgreSQL
{
    public class ServerDataPostgreSQL : DatabaseServerData
    {
        public ServerDataPostgreSQL(ILogger<DatabaseServerData> logger, IOptionsMonitor<DatabaseConfig> options, HealthCheckServer checkServer) : base(logger, checkServer, options)
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

        protected override void AddParameterString(DbCommand cmd, string name, string val)
        {
            NpgsqlCommand sqlcmd = (NpgsqlCommand)cmd;
            sqlcmd.Parameters.Add(name, NpgsqlTypes.NpgsqlDbType.Varchar).Value = val;
        }

        protected override void AddParameterInt(DbCommand cmd, string name, int val)
        {
            NpgsqlCommand sqlcmd = (NpgsqlCommand)cmd;
            sqlcmd.Parameters.Add(name, NpgsqlTypes.NpgsqlDbType.Integer).Value = val;
        }
    }
}
