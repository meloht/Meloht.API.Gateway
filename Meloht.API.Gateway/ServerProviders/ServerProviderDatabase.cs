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
        private readonly DatabaseReadServerData _data;

        public ServerProviderDatabase(DatabaseReadServerData data)
        {
            _data = data;
        }

        public IReadOnlyList<ServerNode> GetHealthServers()
        {
            return _data.GetHealthyServers();
        }

        public IReadOnlyList<ServerNode> GetOriginalServers()
        {
            return _data.GetAllServers();
        }

        public int GetServerWeightSum()
        {
            return _data.GetServerWeightSum();
        }
    }
}
