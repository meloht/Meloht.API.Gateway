using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Meloht.API.ServiceDiscovery.Server.Configuration
{
    public class DatabaseConfig
    {
        [Range(1, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Required]
        public int DatabaseTimeoutSeconds { get; set; } = 5;
        [Required]
        public string ConnectionString { get; set; }
    }
}
