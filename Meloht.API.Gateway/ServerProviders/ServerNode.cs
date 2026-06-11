using Meloht.API.Gateway.LoadBalancing;
using Meloht.API.Gateway.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.ServerProviders
{
    public class ServerNode : ServerNodeConfig
    {
        public ServerHealth Health { get; set; } = ServerHealth.Healthy;
        public int ConcurrentRequestCount
        {
            get => ConcurrencyCounter.Value;
            set => ConcurrencyCounter.Value = value;
        }
        
        internal AtomicCounter ConcurrencyCounter { get; } = new AtomicCounter();
    }
}
