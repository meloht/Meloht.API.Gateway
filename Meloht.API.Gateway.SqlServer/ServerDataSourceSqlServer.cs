using Meloht.API.Gateway.ServerProviders;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace Meloht.API.Gateway.SqlServer
{
    public class ServerDataSourceSqlServer : DatabaseAutoUpdate
    {
        public ServerDataSourceSqlServer(IConfiguration config, ILogger<ServerProviderDatabase> logger) : base(config, logger)
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
