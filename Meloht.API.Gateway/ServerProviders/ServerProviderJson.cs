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
    public sealed class ServerProviderJson : IServerProvider
    {
        private readonly Dictionary<string, ServerNode> _serversDict;
        private readonly List<ServerNode> _serversOriginalList;
        private readonly List<ServerNode> _serversHealthList;

        private readonly object _lock = new();

        private readonly IOptionsMonitor<ServerNodeOptions> _options;
        private readonly HealthCheckServer? _healthCheckServer;
        private readonly ILogger<ServerProviderJson> _log;
        private readonly ParallelOptions _parallelOptions;

        public ServerProviderJson(IOptionsMonitor<ServerNodeOptions> options, IServiceProvider provider, ILogger<ServerProviderJson> logger)
        {
            _options = options;
            _log = logger;
            _options.OnChange(OnConfigChangedAsync);
            _healthCheckServer = provider.GetService<HealthCheckServer>();
            _serversDict = new Dictionary<string, ServerNode>();
            _serversOriginalList = new List<ServerNode>();
            _serversHealthList = new List<ServerNode>();
            _parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
        }

        private void OnConfigChangedAsync(ServerNodeOptions options)
        {
            if (options.Servers != null && options.Servers.Count > 0)
            {

                try
                {
                    List<ServerNode> serverNodes = AppUtils.UpdateData(options.Servers, _serversDict);
                    lock (_lock)
                    {
                        _serversOriginalList.Clear();
                        _serversOriginalList.AddRange(serverNodes);
                    }

                    if (_healthCheckServer != null)
                    {
                        var task = _healthCheckServer.CheckServerHealthAsync(_parallelOptions, serverNodes);
                        Task.WaitAll(task);
                    }

                    lock (_lock)
                    {
                        _serversHealthList.Clear();
                        _serversHealthList.AddRange(serverNodes.Where(p => p.Health == ServerHealth.Healthy));
                    }
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
    }
}
