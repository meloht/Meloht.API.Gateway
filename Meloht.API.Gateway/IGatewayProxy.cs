using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace Meloht.API.Gateway
{
    public interface IGatewayProxy
    {
        Task ProcessRequestAsync(HttpContext httpContext);

        internal void ReturnPool(RequestModel item);

        internal Channel<RequestModel> RequestChannel { get; }
        internal ConcurrentDictionary<Guid, TaskCompletionSource<HttpResponseMessage>> TargetRequestQueue { get; }
    }
}
