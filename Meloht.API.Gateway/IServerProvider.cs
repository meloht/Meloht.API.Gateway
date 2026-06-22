using Meloht.API.Gateway.Common.Configuration;
using Meloht.API.Gateway.Common.HealthCheck;
using Meloht.API.Gateway.ServerProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway
{
    public interface IServerProvider: IServerHealthProvider
    {
        IReadOnlyList<ServerNode> GetHealthServers();

        ServerCluster GetCluster();
    }
}
