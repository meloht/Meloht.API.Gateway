using Meloht.API.Gateway.ServerProviders;
using Meloht.API.Gateway.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Text;

namespace Meloht.API.Gateway.LoadBalancing
{
    internal sealed class RandomLoadBalancingPolicy : ILoadBalancingPolicy
    {
        private readonly IRandomFactory _randomFactory;

        public RandomLoadBalancingPolicy(IRandomFactory randomFactory)
        {
            _randomFactory = randomFactory;
        }

        public string Name => LoadBalancingPolicies.Random;

        public ServerNode? PickDestination(ServerCluster cluster)
        {
            if (cluster == null || cluster.Servers.Length == 0)
            {
                return null;
            }
            var serverNodes = cluster.Servers;
            if (serverNodes.Length == 0)
            {
                return null;
            }

            var random = _randomFactory.CreateRandomInstance();
            return serverNodes[random.Next(serverNodes.Length)];
        }
    }
}
