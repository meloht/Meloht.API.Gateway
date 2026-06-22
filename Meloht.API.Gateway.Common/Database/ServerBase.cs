using Meloht.API.Gateway.Common.Configuration;
using Meloht.API.Gateway.Common.HealthCheck;
using Meloht.API.Gateway.ServerProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Common.Database
{
    public class ServerBase
    {
        protected readonly Dictionary<string, ServerNode> _serversDict;
        protected readonly List<ServerNode> _serversOriginalList;
        protected readonly List<ServerNode> _serversHealthList;
        public readonly HealthCheckServer _healthCheckServer;
        public readonly ServerCluster _serverCluster;
        protected int _weightSum;
        protected readonly object _lock = new();

        public ServerBase(HealthCheckServer healthCheckServer)
        {
            _serversDict = new Dictionary<string, ServerNode>();
            _serversOriginalList = new List<ServerNode>();
            _serversHealthList = new List<ServerNode>();
            _serverCluster = new ServerCluster();
            _healthCheckServer = healthCheckServer;

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
                UpdateHealthlList(serverNodes.Where(p => p.Health == ServerHealth.Healthy));
            }
        }
        private void UpdateHealthlList(IEnumerable<ServerNode> serverNodes)
        {
            _serversHealthList.Clear();
            _serversHealthList.AddRange(serverNodes);
            _weightSum = _serversHealthList.Sum(p => p.Weight);

            _serverCluster.WeightSum = _weightSum;
            _serverCluster.Servers = _serversHealthList.ToArray();
            _serverCluster.WeightIndexArr = new int[_weightSum];
            for (int i = 0, j = 0; i < _serverCluster.Servers.Length; i++)
            {
                var item = _serverCluster.Servers[i];
                for (int k = 0; k < item.Weight; k++)
                {
                    _serverCluster.WeightIndexArr[j++] = i;
                }

            }
        }

        public void UpdateHealthListByHealthService()
        {
            lock (_lock)
            {
                UpdateHealthlList(_serversOriginalList.Where(p => p.Health == ServerHealth.Healthy));
            }
        }


    }
}
