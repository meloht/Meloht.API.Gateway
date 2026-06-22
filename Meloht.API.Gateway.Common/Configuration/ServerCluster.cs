using Meloht.API.Gateway.ServerProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Common.Configuration
{
    public class ServerCluster
    {
        public int WeightSum { get; set; }
        public ServerNode[] Servers { get; set; } = [];
        public int[] WeightIndexArr { get; set; } = [];

    }
}
