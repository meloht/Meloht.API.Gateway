using Meloht.API.Gateway.LoadBalancing;
using Meloht.API.Gateway.Utilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;


namespace Meloht.API.Gateway
{
    internal class AppSettings
    {
        public const string HttpRequestTimeout = "Gateway:HttpRequestTimeout";
        public const string PoolSize = "Gateway:PoolSize";
        public const string LoadBalancingPolicy = "Gateway:LoadBalancingPolicy";
        public const string GatewayClient = "GatewayClient";

        public static string GetLoadBalancingPolicy(IConfiguration configuration)
        {
            string loadBalancingPolicy = configuration.GetValue<string>(LoadBalancingPolicy, LoadBalancingPolicies.RoundRobin);
            return loadBalancingPolicy;
        }

        public static int GetHttpRequestTimeout(IConfiguration configuration)
        {
            int httpRequestTimeout = configuration.GetValue<int>(HttpRequestTimeout, 120);
            return httpRequestTimeout;
        }
        public static int GetPoolSize(IConfiguration configuration)
        {
            int poolSize = configuration.GetValue<int>(PoolSize, 100);
            return poolSize;
        }
    }
}
