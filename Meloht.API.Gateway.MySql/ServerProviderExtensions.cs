using Meloht.API.Gateway.HostServices;
using Meloht.API.Gateway.ServerProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Meloht.API.Gateway.MySql
{
    public static class ServerProviderExtensions 
    {
        private static void AddGatewayServerProviderDatabase(IServiceCollection services)
        {
            services.AddHostedService<DatabaseAutoUpdateService>();
            services.AddSingleton<DatabaseReadServerData, ServerDataSourceMySql>();
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
