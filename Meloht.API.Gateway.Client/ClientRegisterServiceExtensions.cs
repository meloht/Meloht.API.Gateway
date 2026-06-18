using Meloht.API.Gateway.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Client
{
    public static class ClientRegisterServiceExtensions
    {
        public static void AddClientServiceDiscovery(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<GatewayClientConfig>().Bind(configuration.GetSection(GatewayClientConfig.ClientConfigKey)).ValidateDataAnnotations().ValidateOnStart();
            services.AddHostedService<ClientRegisterService>();
            services.AddHttpClient(HttpClientKey.ClientKey);
        }

        public static IApplicationBuilder UseGatewayClient(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HealthCheckMiddleware>();
        }
    }
}
