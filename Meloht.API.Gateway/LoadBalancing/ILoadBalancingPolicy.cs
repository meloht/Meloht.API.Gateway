using Meloht.API.Gateway.ServerProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.LoadBalancing
{
    /// <summary>
    /// Provides a method that applies a load balancing policy to select a destination.
    /// </summary>
    public interface ILoadBalancingPolicy
    {
        /// <summary>
        ///  A unique identifier for this load balancing policy. This will be referenced from config.
        /// </summary>
        string Name { get; }
        ServerNode? PickDestination(ServerCluster cluster);
    }
}
