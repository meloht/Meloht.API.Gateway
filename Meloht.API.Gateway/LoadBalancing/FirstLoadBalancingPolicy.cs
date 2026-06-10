using Meloht.API.Gateway.ServerProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.LoadBalancing
{
    /// <summary>
    /// Select the alphabetically first available destination without considering load. This is useful for dual destination fail-over systems.
    /// </summary>
    internal sealed class FirstLoadBalancingPolicy : ILoadBalancingPolicy
    {
        public string Name => LoadBalancingPolicies.FirstAlphabetical;

        public ServerNode? PickDestination(IReadOnlyList<ServerNode> serverNodes)
        {
            if (serverNodes.Count == 0)
            {
                return null;
            }

            var selectedDestination = serverNodes[0];
            for (var i = 1; i < serverNodes.Count; i++)
            {
                var destination = serverNodes[i];
                if (string.Compare(selectedDestination.UniqueName, destination.UniqueName, StringComparison.OrdinalIgnoreCase) > 0)
                {
                    selectedDestination = destination;
                }
            }

            return selectedDestination;
        }
    }
}
