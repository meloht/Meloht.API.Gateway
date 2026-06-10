using Meloht.API.Gateway.ServerProviders;
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
        public static IServiceCollection AddGatewaySettings(this IServiceCollection services)
        {
            services.AddSingleton<IGatewayProxy, GatewayProxyHandler>();
            services.AddHttpClient(GatewayClient);
            services.AddHostedService<BackgroundForward>();

            return services;
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
