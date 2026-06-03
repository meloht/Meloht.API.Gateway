using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway
{
    public interface IGatewayProxy
    {
        Task ProcessRequestAsync(HttpContext httpContext);
    }
}
