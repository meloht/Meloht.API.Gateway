using Meloht.API.Gateway.HostServices;
using Meloht.API.Gateway.ServerProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Meloht.API.Gateway.PostgreSQL
{
    public static class ServerProviderExtensions
    {

        public static void AddGatewayServerProviderDatabase(IServiceCollection services)
        {
            services.AddHostedService<DatabaseAutoUpdateService>();
            services.AddSingleton<DatabaseReadServerData, ServerDataSourcePostgreSQL>();
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
