using Meloht.API.Gateway.Common.Configuration;
using Meloht.API.Gateway.Common.HealthCheck;
using Meloht.API.Gateway.Configuration;
using Meloht.API.Gateway.ServerProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Json
{
    public static class ServerProviderExtensions
    {
        private const string TargetServersKey = "Gateway:TargetServers";
        public static IServiceCollection AddGatewayService(this IServiceCollection services, IConfiguration configuration)
        {
            AddGatewayServerProviderJson(services, configuration);
            ServiceCollectionExtensions.AddGatewaySettings(services, configuration);
            return services;
        }
        private static void AddGatewayServerProviderJson(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<List<ServerNodeConfig>>(configuration.GetSection(TargetServersKey));
            services.AddSingleton<IServerProvider, ServerProviderJson>();
            services.AddSingleton<IServerHealthProvider>(sp => sp.GetRequiredService<ServerProviderJson>());
        }
    }
}
