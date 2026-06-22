using Meloht.API.Gateway.Common.Configuration;
using Meloht.API.Gateway.Common.HealthCheck;
using Meloht.API.ServiceDiscovery.Server;
using Meloht.API.ServiceDiscovery.Server.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Meloht.API.ServiceDiscovery.Provider.SqlServer
{
    public class ServerDataSqlServer : DatabaseServerData
    {
        public ServerDataSqlServer(ILogger<DatabaseServerData> logger, IOptionsMonitor<DatabaseConfig> options, HealthCheckServer checkServer) : base(logger, checkServer, options)
        {
        }

        protected override void AddParameterInt(DbCommand cmd, string name, int val)
        {
            SqlCommand sqlcmd = (SqlCommand)cmd;
            sqlcmd.Parameters.Add($"@{name}", System.Data.SqlDbType.Int).Value = val;
        }

        protected override void AddParameterString(DbCommand cmd, string name, string val)
        {
            SqlCommand sqlcmd = (SqlCommand)cmd;
            sqlcmd.Parameters.Add($"@{name}", System.Data.SqlDbType.NVarChar).Value = val;
        }

        protected override DbCommand GetDbCommand(string sql, DbConnection connection)
        {
            return new SqlCommand(sql, (SqlConnection)connection);
        }

        protected override DbConnection GetDbConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}
