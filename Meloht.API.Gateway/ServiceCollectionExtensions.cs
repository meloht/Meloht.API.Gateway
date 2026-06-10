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

        public static IServiceCollection AddGatewaySettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IRandomFactory, RandomFactory>();

            AddLoadBalancingPolicy(services, configuration);

            services.AddSingleton<IGatewayProxy, GatewayProxyHandler>();
            services.AddHttpClient(AppSettings.GatewayClient);
            services.AddHostedService<BackgroundForward>();

            return services;
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

        public static IApplicationBuilder UseGateway(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestHandlerMiddleware>();
        }

        public static void AddGatewayServerProviderJson(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ServerNodeOptions>(configuration.GetSection(ServerNodeOptions.TargetServers));
            services.AddSingleton<IServerProvider, ServerProviderJson>();
        }

    }
}
