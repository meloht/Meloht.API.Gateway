using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Client
{
    public static class ClientRegisterServiceExtensions
    {
        public static void AddClientServiceDiscovery(this IServiceCollection services)
        {
            services.AddHostedService<ClientRegisterService>();
        }

        public static IApplicationBuilder UseGatewayClient(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HealthCheckMiddleware>();
        }
    }
}
