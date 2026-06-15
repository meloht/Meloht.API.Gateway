using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Common
{
    public static class ServiceDiscoveryKey
    {
        public const string ServiceDiscoveryHost = "ServiceDiscoveryHost";
        public const string ServiceDiscoveryPort = "ServiceDiscoveryPort";
        public const string RequestTimeoutSeconds = "RequestTimeoutSeconds";
        public const string RegisterPath = "register";
        public const string LogoutPath = "logout";
    }
}
