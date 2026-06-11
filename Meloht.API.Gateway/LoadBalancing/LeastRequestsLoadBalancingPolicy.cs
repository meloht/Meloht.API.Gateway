using Meloht.API.Gateway.ServerProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.LoadBalancing
{
    internal sealed class LeastRequestsLoadBalancingPolicy : ILoadBalancingPolicy
    {
        public string Name => LoadBalancingPolicies.LeastRequests;

        public ServerNode? PickDestination(IReadOnlyList<ServerNode> serverNodes, int weightSum)
        {
            if (serverNodes.Count == 0)
            {
                return null;
            }

            var destinationCount = serverNodes.Count;
            var leastRequestsDestination = serverNodes[0];
            var leastRequestsCount = leastRequestsDestination.ConcurrentRequestCount;
            for (var i = 1; i < destinationCount; i++)
            {
                var destination = serverNodes[i];
                var endpointRequestCount = destination.ConcurrentRequestCount;
                if (endpointRequestCount < leastRequestsCount)
                {
                    leastRequestsDestination = destination;
                    leastRequestsCount = endpointRequestCount;
                }
            }
            return leastRequestsDestination;
        }
    }
}
