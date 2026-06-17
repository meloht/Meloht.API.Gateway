using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Common
{
    public class ServerNodeConfig
    {
        public int Id { get; set; }
        public required string UniqueName { get; set; }
        /// <summary>
        /// ip + port
        /// </summary>
        public required string Host { get; set; }


        /// <summary>
        /// http or https
        /// </summary>
        public required string Protocol { get; set; }

        public int Weight { get; set; }
    }
}
