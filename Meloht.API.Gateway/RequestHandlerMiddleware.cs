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


        //private readonly RequestDelegate _next;

        //public RequestHandlerMiddleware(RequestDelegate next)
        //{
        //    _next = next;
        //}

        //public async Task InvokeAsync(HttpContext context)
        //{
        //    var cultureQuery = context.Request.Query["culture"];
        //    if (!string.IsNullOrWhiteSpace(cultureQuery))
        //    {
        //        var culture = new CultureInfo(cultureQuery);

        //        CultureInfo.CurrentCulture = culture;
        //        CultureInfo.CurrentUICulture = culture;
        //    }

        //    // Call the next delegate/middleware in the pipeline.
        //    await _next(context);
        //}
    }
}
