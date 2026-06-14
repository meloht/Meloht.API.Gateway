using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Meloht.API.Gateway
{
    public class RequestHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        private IGatewayProxy _queueService;
        public RequestHandlerMiddleware(RequestDelegate next, IGatewayProxy queueService)
        {
            _next = next;
            _queueService = queueService;
        }


        public async Task InvokeAsync(HttpContext httpContext)
        {
            await _queueService.ProcessRequestAsync(httpContext);
        }


    }
}
