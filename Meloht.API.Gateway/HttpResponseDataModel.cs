using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Meloht.API.Gateway
{
    internal class HttpResponseDataModel
    {
        public string Guid { get; set; }
        public HttpContext HttpContext { get; set; }
    }

 
}
