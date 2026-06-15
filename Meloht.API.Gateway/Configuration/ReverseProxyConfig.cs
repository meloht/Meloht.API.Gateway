using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Configuration
{
    public class ReverseProxyConfig
    {
        public int RequestQueuePoolSize { get; set; } = 1000;
        public int RequestTimeoutSeconds { get; set; } = 120;
    }
}
