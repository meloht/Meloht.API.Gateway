using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway
{
    public static class ServiceCollectionExtensions
    {
        internal const string GatewayClient = "GatewayClient";
        public static IServiceCollection AddGatewaySettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IGatewayProxy, GatewayProxyForward>();
            services.AddHttpClient(GatewayClient);
            services.Configure<ServerNodeOptions>(configuration.GetSection(ServerNodeOptions.TargetServers));
            return services;
        }

        public static IApplicationBuilder UseGateway(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestHandlerMiddleware>();
        }

        public static void AddGatewayServerProviderJson(this IServiceCollection services)
        {
            services.AddSingleton<IServerProvider, ServerProviderJson>();
        }

    }
}
