using Meloht.API.Gateway.ServerProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Common.HealthCheck
{
    public interface IServerHealthProvider
    {
        IReadOnlyList<ServerNode> GetOriginalServers();

        void UpdateHealthListByHealthService();
    }
}
