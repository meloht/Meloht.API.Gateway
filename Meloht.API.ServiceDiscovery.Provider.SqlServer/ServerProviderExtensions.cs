using Meloht.API.ServiceDiscovery.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.ServiceDiscovery.Provider.SqlServer
{
    public static class ServerProviderExtensions
    {
        public static void AddServiceDiscovery(this IServiceCollection services, IConfiguration configuration)
        {
            ServiceCollectionExtensions.AddDatabaseConfig(services, configuration);
            ServiceCollectionExtensions.AddHealthCheck(services, configuration);
            services.AddSingleton<DatabaseServerData, ServerDataSqlServer>();
        }

        public static IApplicationBuilder UseServiceDiscovery(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ServiceDiscoveryMiddleware>();
        }
    }
}
