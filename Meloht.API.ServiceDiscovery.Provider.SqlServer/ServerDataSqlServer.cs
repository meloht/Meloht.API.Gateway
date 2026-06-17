using Meloht.API.ServiceDiscovery.Server;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Meloht.API.ServiceDiscovery.Provider.SqlServer
{
    public class ServerDataSqlServer : DatabaseServerData
    {
        public ServerDataSqlServer(ILogger<DatabaseServerData> logger) : base(logger)
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
