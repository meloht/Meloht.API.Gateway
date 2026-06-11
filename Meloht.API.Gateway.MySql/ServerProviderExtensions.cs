using Meloht.API.Gateway.HostServices;
using Meloht.API.Gateway.ServerProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Meloht.API.Gateway.MySql
{
    public static class ServerProviderExtensions
    {
        public static void AddServerProviderMySql(this IServiceCollection services)
        {
            services.AddHostedService<DatabaseAutoUpdateService>();
            services.AddSingleton<DatabaseReadServerData, ServerDataSourceMySql>();
            services.AddSingleton<IServerProvider, ServerProviderDatabase>();
        }
    }
}
