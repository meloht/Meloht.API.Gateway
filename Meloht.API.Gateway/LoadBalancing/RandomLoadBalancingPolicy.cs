using Meloht.API.Gateway.ServerProviders;
using Meloht.API.Gateway.Utilities;
using System;
using System.Collections.Generic;
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

        public ServerNode? PickDestination(IReadOnlyList<ServerNode> serverNodes, int weightSum)
        {
            if (serverNodes.Count == 0)
            {
                return null;
            }
            int[] weightIndex=new int[weightSum];
            var random = _randomFactory.CreateRandomInstance();
            return serverNodes[random.Next(serverNodes.Count)];
        }
    }
}
