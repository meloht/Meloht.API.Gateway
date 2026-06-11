using Meloht.API.Gateway.LoadBalancing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.ServerProviders
{
    public class ServerBase
    {
        protected readonly Dictionary<string, ServerNode> _serversDict;
        protected readonly List<ServerNode> _serversOriginalList;
        protected readonly List<ServerNode> _serversHealthList;
        internal readonly HealthCheckServer? _healthCheckServer;
        internal readonly ServerCluster _serverCluster;
        protected int _weightSum;

        protected readonly object _lock = new();

        public ServerBase(IServiceProvider provider)
        {
            _serversDict = new Dictionary<string, ServerNode>();
            _serversOriginalList = new List<ServerNode>();
            _serversHealthList = new List<ServerNode>();
            _serverCluster = new ServerCluster();
            _healthCheckServer = provider.GetService<HealthCheckServer>();
        }

        protected void UpdateOriginalList(List<ServerNode> serverNodes)
        {
            lock (_lock)
            {
                _serversOriginalList.Clear();
                _serversOriginalList.AddRange(serverNodes);
            }
        }

        protected void UpdateHealthlList(List<ServerNode> serverNodes)
        {
            lock (_lock)
            {
                _serversHealthList.Clear();
                _serversHealthList.AddRange(serverNodes.Where(p => p.Health == ServerHealth.Healthy));
                _weightSum = _serversHealthList.Sum(p => p.Weight);

                _serverCluster.WeightSum = _weightSum;
                _serverCluster.Servers = _serversHealthList.ToArray();
                _serverCluster.WeightIndex = new int[_weightSum];
            }
        }

      


    }
}
