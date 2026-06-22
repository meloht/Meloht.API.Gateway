using Meloht.API.Gateway.Common.Configuration;
using Meloht.API.Gateway.ServerProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.LoadBalancing
{
    internal sealed class LeastRequestsLoadBalancingPolicy : ILoadBalancingPolicy
    {
        public string Name => LoadBalancingPolicies.LeastRequests;

        public ServerNode? PickDestination(ServerCluster cluster)
        {
            if (cluster == null || cluster.Servers.Length == 0)
            {
                return null;
            }
            var serverNodes = cluster.Servers;

            var destinationCount = serverNodes.Length;
            var leastRequestsDestination = serverNodes[0];
            float leastRequestsCount = (float)leastRequestsDestination.ConcurrentRequestCount / leastRequestsDestination.Weight;
            for (var i = 1; i < destinationCount; i++)
            {
                var destination = serverNodes[i];
                var endpointRequestCount = (float)destination.ConcurrentRequestCount / destination.Weight;
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
