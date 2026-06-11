using Meloht.API.Gateway.ServerProviders;
using Meloht.API.Gateway.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Meloht.API.Gateway.LoadBalancing
{
    internal sealed class RoundRobinLoadBalancingPolicy : ILoadBalancingPolicy
    {
        public string Name => LoadBalancingPolicies.RoundRobin;

        private readonly AtomicCounter _counters = new();

        public ServerNode? PickDestination(IReadOnlyList<ServerNode> serverNodes, int weightSum)
        {
            if (serverNodes.Count == 0)
            {
                return null;
            }

            int[] weights = new int[serverNodes.Count];
            var offset = _counters.Increment() - 1;

            // Preventing negative indices from being computed by masking off sign.
            // Ordering of index selection is consistent across all offsets.
            // There may be a discontinuity when the sign of offset changes.
            return serverNodes[(offset & 0x7FFFFFFF) % serverNodes.Count];
        }
    }
}
