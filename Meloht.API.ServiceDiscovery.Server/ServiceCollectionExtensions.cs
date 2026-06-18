using Meloht.API.Gateway.Common.Configuration;
using Meloht.API.Gateway.Common.HealthCheck;
using Meloht.API.ServiceDiscovery.Server.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.ServiceDiscovery.Server
{
    public static class ServiceCollectionExtensions
    {

        public static void AddDatabaseConfig(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseConfig>(configuration.GetSection(AppSettings.DatabaseConfigKey));
        }

        public static void AddHealthCheck(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<HealthCheckConfig>(configuration.GetSection(AppSettings.HealthCheckConfigKey));
            services.AddSingleton<HealthCheckServer>();
            services.AddHostedService<ServerHealthCheckService>();
        }
    }
}
