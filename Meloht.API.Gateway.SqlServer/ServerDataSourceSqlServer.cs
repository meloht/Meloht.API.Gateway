using Meloht.API.Gateway.Common.Configuration;
using Meloht.API.Gateway.Common.Database;
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
    public class ServerDataSourceSqlServer : DatabaseServerClient
    {
        public ServerDataSourceSqlServer(ILogger<DatabaseServerClient> logger, HealthCheckServer checkServer, IOptionsMonitor<DatabaseConfig> options) : base(logger, checkServer, options)
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
