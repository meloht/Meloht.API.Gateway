using Meloht.API.Gateway.Common.Configuration;
using Meloht.API.Gateway.Common.Database;
using Meloht.API.Gateway.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Meloht.API.Gateway.ServerProviders
{
    public class ServerProviderDatabase : IServerProvider
    {
        private readonly DatabaseServerClient _data;

        public ServerProviderDatabase(DatabaseServerClient data)
        {
            _data = data;
        }

        public IReadOnlyList<ServerNode> GetHealthServers()
        {
            return _data.GetHealthyServers();
        }

        public IReadOnlyList<ServerNode> GetOriginalServers()
        {
            return _data.GetAllServers();
        }


        public ServerCluster GetCluster()
        {
            return _data._serverCluster;
        }

        public void UpdateHealthListByHealthService()
        {
            _data.UpdateHealthListByHealthService();
        }
    }
}
