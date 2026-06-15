using Meloht.API.Gateway.HostServices;
using Meloht.API.Gateway.ServerProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.SqlServer
{
    public static class ServerProviderExtensions 
    {

        public static void AddGatewayServerProviderDatabase(IServiceCollection services)
        {
            services.AddHostedService<DatabaseAutoUpdateService>();
            services.AddSingleton<DatabaseReadServerData, ServerDataSourceSqlServer>();
            services.AddSingleton<IServerProvider, ServerProviderDatabase>();
        }
        public static IServiceCollection AddGatewayService(this IServiceCollection services, IConfiguration configuration)
        {
            AddGatewayServerProviderDatabase(services);
            ServiceCollectionExtensions.AddGatewaySettings(services, configuration);
            return services;
        }
    }
}
