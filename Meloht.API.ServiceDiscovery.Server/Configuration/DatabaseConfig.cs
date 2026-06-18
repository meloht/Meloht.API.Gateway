using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.ServiceDiscovery.Server.Configuration
{
    public class DatabaseConfig
    {
        public int DatabaseTimeoutSeconds { get; set; } = 5;
        public string? ConnectionString { get; set; }
    }
}
