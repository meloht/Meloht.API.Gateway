using Meloht.API.Gateway.Common.HealthCheck;
using Meloht.API.Gateway.Configuration;
using Meloht.API.Gateway.ServerProviders;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data.Common;

namespace Meloht.API.Gateway.SqlServer
{
    public class ServerDataSourceSqlServer : DatabaseReadServerData
    {
        public ServerDataSourceSqlServer(ILogger<DatabaseReadServerData> logger, HealthCheckServer checkServer, IOptionsMonitor<DatabaseAutoUpdateConfig> options) : base(logger, checkServer, options)
        {
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
