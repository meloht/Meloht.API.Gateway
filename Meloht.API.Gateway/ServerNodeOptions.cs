using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway
{
    public class ServerNodeOptions
    {
        public const string TargetServers = "TargetServers";

        public ServerNodeConfig[] Servers { get; set; }
    }
}
