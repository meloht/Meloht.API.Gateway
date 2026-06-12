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
        private const string HttpRequestTimeout = "Gateway:HttpRequestTimeout";
        private const string PoolSize = "Gateway:PoolSize";
        private const string LoadBalancingPolicy = "Gateway:LoadBalancingPolicy";
        private const string ConnectionString = "Gateway:DatabaseAutoUpdate:ConnectionString";
        private const string DatabaseAutoUpdateIntervalSeconds = "Gateway:DatabaseAutoUpdate:IntervalSeconds";
        private const string DatabaseTimeoutSeconds = "Gateway:DatabaseAutoUpdate:DatabaseTimeoutSeconds";
        private const string HealthIntervalSeconds = "Gateway:HealthCheck:IntervalSeconds";
        private const string HealthRequestTimeoutSeconds = "Gateway:HealthCheck:RequestTimeoutSeconds";

        public const string TargetServersKey = "Gateway:TargetServers";

        public const string GatewayClient = "GatewayClient";

        public static string GetLoadBalancingPolicy(IConfiguration configuration)
        {
            string loadBalancingPolicy = configuration.GetValue<string>(LoadBalancingPolicy, LoadBalancingPolicies.RoundRobin);
            return loadBalancingPolicy;
        }
        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>(ConnectionString);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Connection string is not configured.");
            }
            return connectionString;
        }
        public static int GetDatabaseAutoUpdateIntervalSeconds(IConfiguration configuration)
        {
            var intervalSeconds = configuration.GetValue<int>(DatabaseAutoUpdateIntervalSeconds, 120);
            return intervalSeconds;
        }
        public static int GetDatabaseTimeoutSeconds(IConfiguration configuration)
        {
            var timeoutSeconds = configuration.GetValue<int>(DatabaseTimeoutSeconds, 5);
            return timeoutSeconds;
        }
        public static int GetHealthIntervalSeconds(IConfiguration configuration)
        {
            var intervalSeconds = configuration.GetValue<int>(HealthIntervalSeconds, 10);
            return intervalSeconds;
        }
        public static int GetHealthRequestTimeoutSeconds(IConfiguration configuration)
        {
            var timeoutSeconds = configuration.GetValue<int>(HealthRequestTimeoutSeconds, 5);
            return timeoutSeconds;
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
