using Meloht.API.Gateway.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;


namespace Meloht.API.Gateway
{
    internal class AppSettingsClient
    {

        public AppSettingsClient(IConfiguration config)
        {
            targetServer = config["TargetServer"];
            httpRequestTimeout = AppUtils.ConvertInt(config["HttpRequestTimeout"], 200);
            poolSize = AppUtils.ConvertInt(config["PoolSize"], 100);
        }

        private int poolSize;
        private int httpRequestTimeout;
        private string targetServer;

        public string TargetServer
        {
            get { return targetServer; }
        }

        public int HttpRequestTimeout
        {
            get { return httpRequestTimeout; }
        }

        public int PoolSize
        {
            get { return poolSize; }
        }
    }
}
