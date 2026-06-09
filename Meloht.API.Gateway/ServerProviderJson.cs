using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway
{
    public class ServerProviderJson : IServerProvider
    {
        private Dictionary<string, ServerNode> _serversDict;
        private readonly List<ServerNode> _serversList;
        private readonly ILogger<ServerProviderJson> _logger;

        private readonly TimeSpan _interval = TimeSpan.FromSeconds(10);
        private readonly object _lock = new();

        private readonly IOptionsMonitor<ServerNodeOptions> _options;

        public ServerProviderJson(IOptionsMonitor<ServerNodeOptions> options, ILogger<ServerProviderJson> logger)
        {
            _options = options;
            _options.OnChange(OnConfigChanged);
            _logger = logger;
            _serversDict = new Dictionary<string, ServerNode>();
            _serversList = new List<ServerNode>();
        }

        private void OnConfigChanged(ServerNodeOptions options)
        {
            if(options.Servers != null)
            {
                lock (_lock) 
                {
                    HashSet<string> updatedAddresses = new HashSet<string>();
                    _serversList.Clear();
                    foreach (var item in options.Servers)
                    {
                        updatedAddresses.Add(item.Address);

                        if (_serversDict.TryGetValue(item.Address, out var node))
                        {
                            node.Weight = item.Weight;
                            node.Name = item.Name;
                            node.Address = item.Address;
                            node.Id = item.Id;
                            _serversList.Add(node);
                        }
                        else
                        {
                            var nodeNew = new ServerNode
                            {
                                Id = item.Id,
                                Name = item.Name,
                                Address = item.Address,
                                Weight = item.Weight
                            };
                            _serversList.Add(nodeNew);
                            _serversDict.Add(item.Address, nodeNew);
                        }
                    }
                    List<string> toRemove = new List<string>();
                    foreach (var item in _serversDict.Keys)
                    {
                        if (!updatedAddresses.Contains(item))
                        {
                            toRemove.Add(item);
                        }
                    }

                    foreach (var item in toRemove)
                    {
                        _serversDict.Remove(item);
                    }
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

        public void Dispose()
        {

        }




    }
}
