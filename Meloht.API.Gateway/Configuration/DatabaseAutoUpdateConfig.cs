using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Configuration
{
    public class DatabaseAutoUpdateConfig
    {
        public int IntervalSeconds { get; set; } = 120;
        public int DatabaseTimeoutSeconds { get; set; } = 5;
        public string? ConnectionString { get; set; }
    }
}
