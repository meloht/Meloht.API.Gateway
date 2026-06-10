using Meloht.API.Gateway.LoadBalance;
using Meloht.API.Gateway.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.ServerProviders
{
    public class ServerNode : ServerNodeConfig
    {
        public ServerHealth Health { get; set; } = ServerHealth.Unknown;
        public int ConcurrentRequestCount
        {
            get => ConcurrencyCounter.Value;
            set => ConcurrencyCounter.Value = value;
        }
        
        internal AtomicCounter ConcurrencyCounter { get; } = new AtomicCounter();
    }
}
