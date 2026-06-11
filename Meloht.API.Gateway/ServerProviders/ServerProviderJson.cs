using Meloht.API.Gateway.LoadBalancing;
using Meloht.API.Gateway.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.ServerProviders
{
    public sealed class ServerProviderJson : ServerBase, IServerProvider
    {
        private readonly IOptionsMonitor<ServerNodeOptions> _options;
        private readonly ILogger<ServerProviderJson> _log;
        private readonly ParallelOptions _parallelOptions;

        public ServerProviderJson(IOptionsMonitor<ServerNodeOptions> options, IServiceProvider provider, ILogger<ServerProviderJson> logger) : base(provider)
        {
            _options = options;
            _log = logger;
            _options.OnChange(OnConfigChanged);
            _parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
            OnConfigChanged(_options.CurrentValue);
        }

        private void OnConfigChanged(ServerNodeOptions options)
        {
            if (options.Servers != null && options.Servers.Count > 0)
            {
                try
                {
                    List<ServerNode> serverNodes = AppUtils.UpdateData(options.Servers, _serversDict);

                    UpdateOriginalList(serverNodes);

                    if (_healthCheckServer != null)
                    {
                        var task = _healthCheckServer.CheckServerHealthAsync(_parallelOptions, serverNodes);
                        Task.WaitAll(task);
                    }

                    UpdateHealthlList(serverNodes);
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Error occurred while updating server configuration.");
                }

            }
        }

        public IReadOnlyList<ServerNode> GetHealthServers()
        {
            return _serversHealthList;
        }

        public IReadOnlyList<ServerNode> GetOriginalServers()
        {
            return _serversOriginalList;
        }

        public ServerCluster GetCluster()
        {
            return _serverCluster;
        }
    }
}
