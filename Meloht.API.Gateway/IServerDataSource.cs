using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway
{
    public interface IServerDataSource
    {
        List<ServerNodeConfig> GetServerNodes(string connectionString);
    }
}
