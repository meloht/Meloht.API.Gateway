using Meloht.API.Gateway.Common;
using Meloht.API.Gateway.Common.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;

namespace Meloht.API.ServiceDiscovery.Server
{
    public class ServiceDiscoveryMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ServiceDiscoveryMiddleware> _logger;
        private readonly DatabaseServerData _databaseServer;
        public ServiceDiscoveryMiddleware(RequestDelegate next, ILogger<ServiceDiscoveryMiddleware> logger, DatabaseServerData databaseServer)
        {
            _next = next;
            _logger = logger;
            _databaseServer = databaseServer;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            if (path != null && path.StartsWith(ServiceDiscoveryKey.RegisterPath))
            {
                _logger.LogInformation("ServiceDiscovery Server Received request for path: {Path}", path);

                await ClientRegisterAsync(context, path);

                return; // 关键：直接结束，不再进入后续 pipeline
            }

            await _next(context);
        }

        private async Task ClientRegisterAsync(HttpContext context, string path)
        {
            var clientNode = await JsonHelper.ReadJsonAsync<ServerNodeConfig>(context.Request.Body);
            if (clientNode == null)
            {
                await context.WriteResponseErrorAsync("clientNode is null");
                return;
            }
            clientNode.Host = context.Request.Host.ToString();
            var rent = await _databaseServer.RegisterSaveAndUpdateAsync(clientNode);
            if (rent)
            {
                await context.WriteResponseSuccessAsync("Client register success");
                return;
            }
            else
            {
                await context.WriteResponseErrorAsync("Client register failed");
                return;
            }
          
        }


    }
}
