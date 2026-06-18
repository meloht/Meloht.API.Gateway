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
        private const string RequestTimeoutSeconds = "Gateway:RequestTimeoutSeconds";
        private const string PoolSize = "Gateway:RequestQueuePoolSize";
        private const string LoadBalancingPolicy = "Gateway:LoadBalancingPolicy";
        private const string ConnectionString = "Gateway:DatabaseAutoUpdate:ConnectionString";
        private const string DatabaseAutoUpdateIntervalSeconds = "Gateway:DatabaseAutoUpdate:IntervalSeconds";
        private const string DatabaseTimeoutSeconds = "Gateway:DatabaseAutoUpdate:DatabaseTimeoutSeconds";
        private const string HealthIntervalSeconds = "Gateway:HealthCheck:IntervalSeconds";
        private const string HealthRequestTimeoutSeconds = "Gateway:HealthCheck:RequestTimeoutSeconds";

        public const string TargetServersKey = "Gateway:TargetServers";
        public const string ReverseProxyKey = "Gateway:ReverseProxy";
        public const string HealthCheckKey = "Gateway:HealthCheck";
        public const string DatabaseAutoUpdateKey = "Gateway:DatabaseAutoUpdate";

        internal const int ProxyRequestTimeoutSeconds = 120;
        internal const int ProxyRequestQueuePoolSize = 1000;
        internal const int DatabaseIntervalSeconds = 120;
        internal const int DatabaseExecuteTimeoutSeconds = 5;




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
            var intervalSeconds = configuration.GetValue<int>(DatabaseAutoUpdateIntervalSeconds, DatabaseIntervalSeconds);
            return intervalSeconds;
        }
        public static int GetDatabaseTimeoutSeconds(IConfiguration configuration)
        {
            var timeoutSeconds = configuration.GetValue<int>(DatabaseTimeoutSeconds, DatabaseExecuteTimeoutSeconds);
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
            int httpRequestTimeout = configuration.GetValue<int>(RequestTimeoutSeconds, ProxyRequestTimeoutSeconds);
            return httpRequestTimeout;
        }
        public static int GetPoolSize(IConfiguration configuration)
        {
            int poolSize = configuration.GetValue<int>(PoolSize, ProxyRequestQueuePoolSize);
            return poolSize;
        }


    }
}
