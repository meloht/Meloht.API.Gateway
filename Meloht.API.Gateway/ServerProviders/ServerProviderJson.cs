using Meloht.API.Gateway.Common;
using Meloht.API.Gateway.Configuration;
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
        private readonly IOptionsMonitor<List<ServerNodeConfig>> _options;
        private readonly ILogger<ServerProviderJson> _log;
        private readonly ParallelOptions _parallelOptions;

        public ServerProviderJson(IOptionsMonitor<List<ServerNodeConfig>> options, HealthCheckServer checkServer, ILogger<ServerProviderJson> logger) : base(checkServer)
        {
            _options = options;
            _log = logger;
            _options.OnChange(OnConfigChanged);
            _parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
            OnConfigChanged(_options.CurrentValue);
        }

        private void OnConfigChanged(List<ServerNodeConfig> options)
        {
            if (options != null && options.Count > 0)
            {
                try
                {
                    List<ServerNode> serverNodes = AppUtils.UpdateData(options, _serversDict);

                    UpdateOriginalList(serverNodes);

                    var task = _healthCheckServer.CheckServerHealthAsync(_parallelOptions, serverNodes);
                    Task.WaitAll(task);

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
