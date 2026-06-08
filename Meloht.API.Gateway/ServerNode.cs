using Meloht.API.Gateway.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway
{
    public class ServerNode
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }

        public int Weight { get; set; }

        public int ConcurrentRequestCount
        {
            get => ConcurrencyCounter.Value;
            set => ConcurrencyCounter.Value = value;
        }

        internal AtomicCounter ConcurrencyCounter { get; } = new AtomicCounter();
    }
}
