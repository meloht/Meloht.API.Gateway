using Meloht.API.ServiceDiscovery.Server;
using Meloht.API.ServiceDiscovery.Server.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Meloht.API.ServiceDiscovery.Provider.MySql
{
    public class ServerDataMySql : DatabaseServerData
    {
        public ServerDataMySql(ILogger<DatabaseServerData> logger, IOptionsMonitor<DatabaseConfig> options) : base(logger, options)
        {
        }

        protected override DbCommand GetDbCommand(string sql, DbConnection connection)
        {
            return new MySqlCommand(sql, (MySqlConnection)connection);
        }

        protected override void AddParameterString(DbCommand cmd, string name, string val)
        {
            MySqlCommand mySqlcmd = (MySqlCommand)cmd;
            mySqlcmd.Parameters.Add($"@{name}", MySqlDbType.VarChar).Value = val;
        }

        protected override void AddParameterInt(DbCommand cmd, string name, int val)
        {
            MySqlCommand mySqlcmd = (MySqlCommand)cmd;
            mySqlcmd.Parameters.Add($"@{name}", MySqlDbType.Int32).Value = val;
        }

        protected override DbConnection GetDbConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

       
    }
}
