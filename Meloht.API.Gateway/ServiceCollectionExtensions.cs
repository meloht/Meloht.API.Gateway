using Meloht.API.Gateway.Common;
using Meloht.API.Gateway.Common.Configuration;
using Meloht.API.Gateway.Common.HealthCheck;
using Meloht.API.Gateway.Common.HostServices;
using Meloht.API.Gateway.Configuration;
using Meloht.API.Gateway.HostServices;
using Meloht.API.Gateway.LoadBalancing;
using Meloht.API.Gateway.ServerProviders;
using Meloht.API.Gateway.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Meloht.API.Gateway
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGatewaySettings(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ReverseProxyConfig>(configuration.GetSection(AppSettings.ReverseProxyKey));
          
            services.AddSingleton<IRandomFactory, RandomFactory>();
            services.AddHttpClient(HttpClientKey.ClientKey);

            AddLoadBalancingPolicy(services, configuration);

            services.AddSingleton<IGatewayProxy, GatewayProxyHandler>();

            services.AddHostedService<BackgroundForwardService>();
            AddHealthCheck(services, configuration);

        }
        public static IApplicationBuilder UseGateway(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestHandlerMiddleware>();
        }


        public static void AddDatabaseProvider(IServiceCollection services)
        {
            services.AddSingleton<IServerProvider, ServerProviderDatabase>();
            services.AddSingleton<IServerHealthProvider>(sp => sp.GetRequiredService<ServerProviderDatabase>());
        }
        public static void AddDatabaseConfig(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseConfig>(configuration.GetSection(AppSettings.TargetServersKey));
        }

        private static void AddHealthCheck(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<HealthCheckConfig>(configuration.GetSection(AppSettings.HealthCheckKey));
            services.AddSingleton<HealthCheckServer>();
            services.AddHostedService<ServerHealthCheckService>();
        }
        private static void AddLoadBalancingPolicy(IServiceCollection services, IConfiguration configuration)
        {
            string loadBalancingPolicy = AppSettings.GetLoadBalancingPolicy(configuration);

            if (loadBalancingPolicy == LoadBalancingPolicies.RoundRobin)
            {
                services.AddSingleton<ILoadBalancingPolicy, RoundRobinLoadBalancingPolicy>();
            }
            else if (loadBalancingPolicy == LoadBalancingPolicies.Random)
            {
                services.AddSingleton<ILoadBalancingPolicy, RandomLoadBalancingPolicy>();
            }
            else if (loadBalancingPolicy == LoadBalancingPolicies.FirstAlphabetical)
            {
                services.AddSingleton<ILoadBalancingPolicy, FirstLoadBalancingPolicy>();
            }
            else if (loadBalancingPolicy == LoadBalancingPolicies.LeastRequests)
            {
                services.AddSingleton<ILoadBalancingPolicy, LeastRequestsLoadBalancingPolicy>();
            }
            else if (loadBalancingPolicy == LoadBalancingPolicies.PowerOfTwoChoices)
            {
                services.AddSingleton<ILoadBalancingPolicy, PowerOfTwoChoicesLoadBalancingPolicy>();
            }
            else
            {
                throw new InvalidOperationException($"Unsupported load balancing policy: {loadBalancingPolicy}");
            }

        }



    }
}
