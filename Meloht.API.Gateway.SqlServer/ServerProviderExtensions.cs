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
        public static void AddServerProviderSqlServer(this IServiceCollection services)
        {
            services.AddHostedService<ServerDataSourceSqlServer>();
            services.AddSingleton<IServerProvider, ServerProviderDatabase>();
        }
    }
}
