using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGatewaySettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ServerNodeOptions>(configuration.GetSection(ServerNodeOptions.TargetServers));
            return services;
        }
    }
}
