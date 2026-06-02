using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway
{
    internal class HttpRequestModel
    {
        public string ContentType { get; set; }
        public bool IsHttps { get; set; }

        public string Method { get; set; }

        public string Body { get; set; }

        public Dictionary<string, string[]> FormValues { get; set; }

        public string Guid { get; set; }

        public string TargetUrl { get; set; }

        public Dictionary<string, string[]> QueryStringValues
        {
            get; set;
        }
    }
}
