using Meloht.API.Gateway.Common.Configuration;
using Meloht.API.Gateway.Common.Database;
using Meloht.API.Gateway.Common.HealthCheck;
using Meloht.API.Gateway.ServerProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.ServiceDiscovery
{
    public static class ServerProviderExtensions
    {
        private const string TargetServersKey = "Gateway:ServiceDiscovery";
        public static IServiceCollection AddGatewayService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<ServiceDiscoveryConfig>().Bind(configuration.GetSection(TargetServersKey)).ValidateDataAnnotations().ValidateOnStart();
            services.AddSingleton<ServiceDiscoveryClient>();
            services.AddHostedService<ServiceDiscoveryHost>();
            services.AddSingleton<IServerProvider, ServerProviderServiceDiscovery>();
            services.AddSingleton<IServerHealthProvider>(sp => sp.GetRequiredService<ServerProviderServiceDiscovery>());

            ServiceCollectionExtensions.AddGatewaySettings(services, configuration);
            return services;
        }

    }
}
