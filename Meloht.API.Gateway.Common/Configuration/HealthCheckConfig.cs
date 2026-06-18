using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Common.Configuration
{
    public class HealthCheckConfig
    {
        public int IntervalSeconds { get; set; } = 10;
        public int RequestTimeoutSeconds { get; set; } = 5;

    }
}
