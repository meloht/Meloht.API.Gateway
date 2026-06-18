using Meloht.API.Gateway.Common.HealthCheck;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;

namespace Meloht.API.Gateway.Client
{
    public class HealthCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HealthCheckMiddleware> _logger;
        public HealthCheckMiddleware(RequestDelegate next, ILogger<HealthCheckMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;
          
            if (path != null && path.StartsWith(HealthCheckAPI.HealthCheckPath))
            {
                _logger.LogInformation("Received request for path: {Path}", path);
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = MediaTypeNames.Application.Json;

                var result = new
                {
                    Message = "OK",
                    Path = path,
                    Host = context.Request.Host,
                };
                string json = System.Text.Json.JsonSerializer.Serialize(result);
                await context.Response.WriteAsync(json);

                return; // 关键：直接结束，不再进入后续 pipeline
            }

            await _next(context);
        }
    }
}
