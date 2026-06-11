using Meloht.API.Gateway.ServerProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway
{
    public interface IServerProvider
    {

        IReadOnlyList<ServerNode> GetHealthServers();

        IReadOnlyList<ServerNode> GetOriginalServers();

    }
}
