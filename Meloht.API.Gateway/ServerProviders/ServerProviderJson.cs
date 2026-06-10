using Meloht.API.Gateway.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.ServerProviders
{
    public class ServerProviderJson : IServerProvider
    {
        private readonly Dictionary<string, ServerNode> _serversDict;
        private readonly List<ServerNode> _serversList;

        private readonly object _lock = new();

        private readonly IOptionsMonitor<ServerNodeOptions> _options;

        public ServerProviderJson(IOptionsMonitor<ServerNodeOptions> options)
        {
            _options = options;
            _options.OnChange(OnConfigChanged);
            _serversDict = new Dictionary<string, ServerNode>();
            _serversList = new List<ServerNode>();
        }

        private void OnConfigChanged(ServerNodeOptions options)
        {
            if (options.Servers != null && options.Servers.Count > 0)
            {
                lock (_lock)
                {
                    AppUtils.UpdateData(options.Servers, _serversDict, _serversList);
                }
            }
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
