using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.ServerProviders
{
    public class ServerNodeConfig
    {

        public int Id { get; set; }
        public required string UniqueName { get; set; }

        public required int Port { get; set; }
        public required string Host { get; set; }
        public string Address { get; set; }

        public int Weight { get; set; }
    }
}
