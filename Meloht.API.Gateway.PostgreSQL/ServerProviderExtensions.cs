using Meloht.API.Gateway.HostServices;
using Meloht.API.Gateway.ServerProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Meloht.API.Gateway.PostgreSQL
{
    public static class ServerProviderExtensions
    {
        public static void AddServerProviderPostgreSQL(this IServiceCollection services)
        {
            services.AddHostedService<DatabaseAutoUpdateService>();
            services.AddSingleton<DatabaseReadServerData, ServerDataSourcePostgreSQL>();
            services.AddSingleton<IServerProvider, ServerProviderDatabase>();
        }
    }
}
