using Meloht.API.Gateway.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Meloht.API.Gateway.ServerProviders
{
    public class ServerProviderDatabase : IServerProvider
    {

        private readonly ILogger<ServerProviderDatabase> _logger;

        private readonly Dictionary<string, ServerNode> _serversDict;
        private readonly List<ServerNode> _serversList;
        private readonly object _lock = new();
        public ServerProviderDatabase(ILogger<ServerProviderDatabase> logger)
        {
            _serversDict = new Dictionary<string, ServerNode>();
            _serversList = new List<ServerNode>();

            _logger = logger;
        }


        public IReadOnlyList<ServerNode> GetServers()
        {
            lock (_lock)
            {
                return _serversList;
            }
        }



    }
}
