using Microsoft.Data.SqlClient;

namespace Meloht.API.Gateway.SqlServer
{
    public class ServerDataSourceSqlServer : IServerDataSource
    {
        public List<ServerNodeConfig> GetServerNodes(string connectionString)
        {
            return null;
        }
    }
}
