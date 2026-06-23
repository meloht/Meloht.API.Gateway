using Meloht.API.Gateway.Common.Configuration;
using Meloht.API.Gateway.Common.Database;
using Meloht.API.Gateway.Common.HealthCheck;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.ServerProviders
{
    public class ServerProviderServiceDiscovery : IServerProvider
    {
        private ServiceDiscoveryClient _data;
        public ServerProviderServiceDiscovery(ServiceDiscoveryClient data)
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
