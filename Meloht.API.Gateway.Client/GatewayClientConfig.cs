using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Meloht.API.Gateway.Client
{
    public class GatewayClientConfig
    {
        public const string ClientConfigKey= "GatewayClient";
        [Required]
        public string? ClientProtocol { get; set; }
        [Required]
        public string? ClientUniqueName { get; set; }

        [Range(1, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Required]
        public int ClientWeight { get; set; }
        [Required]
        public string? ServiceDiscoveryProtocol { get; set; }
        [Required]
        public string? ServiceDiscoveryHost { get; set; }

        [Range(1, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Required]
        public int RequestTimeoutSeconds { get; set; }
    }
}
