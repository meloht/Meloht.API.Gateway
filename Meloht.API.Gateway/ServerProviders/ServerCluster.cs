using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.ServerProviders
{
    public class ServerCluster
    {
        public int WeightSum { get; set; }
        public ServerNode[] Servers { get; set; } = [];
        public int[] WeightIndex { get; set; } = [];

    }
}
