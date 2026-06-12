using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Client
{
    public class HealthCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public HealthCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            if (path != null && path.StartsWith("/health/check"))
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = "application/json";

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
