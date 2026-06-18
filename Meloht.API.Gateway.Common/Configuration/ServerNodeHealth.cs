using Meloht.API.Gateway.ServerProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Common.Configuration
{
    public class ServerNodeHealth: ServerNodeConfig
    {
        public ServerHealth Health { get; set; } = ServerHealth.Unhealthy;
    }
}
