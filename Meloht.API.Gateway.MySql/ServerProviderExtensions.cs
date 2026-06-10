using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Meloht.API.Gateway.MySql
{
    public static class ServerProviderExtensions
    {
        public static void AddServerProviderMySql(this IServiceCollection services)
        {
            services.AddSingleton<IServerProvider, ServerDataSourceMySql>();
        }
    }
}
