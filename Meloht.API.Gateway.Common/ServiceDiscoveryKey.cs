using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Common
{
    public static class ServiceDiscoveryKey
    {
        public const string ServiceDiscoveryHostKey = "ServiceDiscoveryHost";
        public const string ServiceDiscoveryProtocolKey = "Protocol";
        public const string RequestTimeoutSecondsKey = "RequestTimeoutSeconds";
      
        public const string RegisterPath = "/register";
        public const string UnregisterPath = "/unregister";
    }
}
