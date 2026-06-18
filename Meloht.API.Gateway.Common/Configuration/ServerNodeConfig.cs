using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Common.Configuration
{
    public class ServerNodeConfig
    {
        public int Id { get; set; }
        public string UniqueName { get; set; }
        /// <summary>
        /// ip + port
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// http or https
        /// </summary>
        public string Protocol { get; set; }

        public int Weight { get; set; }

      


    }
}
