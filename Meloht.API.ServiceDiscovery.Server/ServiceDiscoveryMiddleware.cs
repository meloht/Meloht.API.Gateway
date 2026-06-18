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
        public ServiceDiscoveryMiddleware(RequestDelegate next, ILogger<ServiceDiscoveryMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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
            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = MediaTypeNames.Application.Json;
            var res = JsonHelper.ReadJsonAsync<ServerNodeConfig>(context.Request.Body);
            var result = new
            {
                Message = "OK",
                Path = path,
                Host = context.Request.Host,
            };

          
            string json = System.Text.Json.JsonSerializer.Serialize(result);
            await context.Response.WriteAsync(json);
        }
    }
}
