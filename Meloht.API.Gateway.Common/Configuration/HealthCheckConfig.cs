using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Meloht.API.Gateway.Common.Configuration
{
    public class HealthCheckConfig
    {
        [Range(1, 120, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Required]
        public int IntervalSeconds { get; set; } = 10;

        [Range(1, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Required]
        public int RequestTimeoutSeconds { get; set; } = 5;

    }
}
