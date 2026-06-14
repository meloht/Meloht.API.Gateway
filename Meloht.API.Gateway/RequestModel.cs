using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway
{
    public class RequestModel 
    {
        public Guid Guid { get; set; }
        public HttpContext Context { get; set; }
        public string ContentType { get; set; }

        public RequestModel(Guid guid, HttpContext context)
        {
            Guid = guid;
            Context = context;
        }

     
    }
}
