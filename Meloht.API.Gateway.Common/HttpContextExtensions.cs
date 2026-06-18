using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;

namespace Meloht.API.Gateway.Common
{
    public static class HttpContextExtensions
    {
        public static async Task WriteResponseSuccessAsync(this HttpContext context, string msg)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = MediaTypeNames.Application.Json;
            var res = ResponseMessage.Success(msg);
            string json = System.Text.Json.JsonSerializer.Serialize(res);
            await context.Response.WriteAsync(json);
        }
        public static async Task WriteResponseErrorAsync(this HttpContext context, string msg)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = MediaTypeNames.Application.Json;
            var res = ResponseMessage.Success(msg);
            string json = System.Text.Json.JsonSerializer.Serialize(res);
            await context.Response.WriteAsync(json);
        }
    }
}
