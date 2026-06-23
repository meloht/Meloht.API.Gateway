using Meloht.API.Gateway.Common.Configuration;
using Meloht.API.Gateway.Common.Database;
using Meloht.API.Gateway.Common.HealthCheck;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.ServerProviders
{
    public class ServerProviderServiceDiscovery : ServerBase, IServerProvider
    {
        public ServerProviderServiceDiscovery(HealthCheckServer healthCheckServer) : base(healthCheckServer)
        {
        }

        public ServerCluster GetCluster()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<ServerNode> GetHealthServers()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<ServerNode> GetOriginalServers()
        {
            throw new NotImplementedException();
        }
    }
}
