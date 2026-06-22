using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Meloht.API.Gateway.Common.Configuration
{
    public class DatabaseConfig
    {

        public int DatabaseTimeoutSeconds { get; set; } = 5;


        public int IntervalSeconds { get; set; } = 120;

        public string? ConnectionString { get; set; }
    }
}
