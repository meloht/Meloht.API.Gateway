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
        public static IServiceCollection AddGatewayService(this IServiceCollection services, IConfiguration configuration)
        {
            ServiceCollectionExtensions.AddGatewayServerProviderJson(services, configuration);
            ServiceCollectionExtensions.AddGatewaySettings(services, configuration);
            return services;
        }
    }
}
