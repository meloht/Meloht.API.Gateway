using Meloht.API.Gateway.ServerProviders;
using Meloht.API.Gateway.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.LoadBalancing
{
    internal sealed class PowerOfTwoChoicesLoadBalancingPolicy : ILoadBalancingPolicy
    {
        private readonly IRandomFactory _randomFactory;

        public PowerOfTwoChoicesLoadBalancingPolicy(IRandomFactory randomFactory)
        {
            _randomFactory = randomFactory;
        }

        public string Name => LoadBalancingPolicies.PowerOfTwoChoices;

        public ServerNode? PickDestination(IReadOnlyList<ServerNode> serverNodes)
        {
            var destinationCount = serverNodes.Count;
            if (destinationCount == 0)
            {
                return null;
            }

            if (destinationCount == 1)
            {
                return serverNodes[0];
            }

            // Pick two, and then return the least busy. This avoids the effort of searching the whole list, but
            // still avoids overloading a single destination.
            var random = _randomFactory.CreateRandomInstance();
            var firstIndex = random.Next(destinationCount);
            int secondIndex;
            do
            {
                secondIndex = random.Next(destinationCount);
            } while (firstIndex == secondIndex);
            var first = serverNodes[firstIndex];
            var second = serverNodes[secondIndex];
            return (first.ConcurrentRequestCount <= second.ConcurrentRequestCount) ? first : second;
        }
    }
}
