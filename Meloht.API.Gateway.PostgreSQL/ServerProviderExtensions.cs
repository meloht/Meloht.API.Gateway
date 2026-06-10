using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Meloht.API.Gateway.PostgreSQL
{
    public static class ServerProviderExtensions
    {
        public static void AddServerProviderPostgreSQL(this IServiceCollection services)
        {
            services.AddSingleton<IServerProvider, ServerDataSourcePostgreSQL>();
        }
    }
}
