using Meloht.API.Gateway.Common;
using Meloht.API.Gateway.Common.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Client
{
    public class ClientRegisterService : IHostedService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ClientRegisterService> _logger;
        private readonly IOptionsMonitor<GatewayClientConfig> _options;
        private GatewayClientConfig _gatewayConfig;

        public ClientRegisterService(IHttpClientFactory httpClientFactory, IOptionsMonitor<GatewayClientConfig> options, ILogger<ClientRegisterService> logger)
        {
            _httpClient = httpClientFactory.CreateClient(HttpClientKey.ClientKey);
            _options = options;
            _options.OnChange(OnConfigChanged);
            _logger = logger;
            _gatewayConfig = _options.CurrentValue;

        }
        private void OnConfigChanged(GatewayClientConfig options)
        {
            _gatewayConfig = options;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_gatewayConfig.ServiceDiscoveryHost))
            {
                _logger.LogError("service discovery host is null");
                throw new ArgumentException("service discovery host is null");
            }
            if (string.IsNullOrEmpty(_gatewayConfig.ServiceDiscoveryProtocol))
            {
                _logger.LogError("service discovery protocol is null");
                throw new ArgumentException("service discovery protocol is null");
            }

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(_gatewayConfig.RequestTimeoutSeconds));
            string url = $"{_gatewayConfig.ServiceDiscoveryProtocol}://{_gatewayConfig.ServiceDiscoveryHost}{ServiceDiscoveryKey.RegisterPath}";
            using var res = await _httpClient.GetAsync(url, cts.Token);

            if (res.IsSuccessStatusCode)
            {
                _logger.LogInformation("The client has registered successfully");
            }
            else
            {
                _logger.LogInformation("The client registration failed");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(_gatewayConfig.RequestTimeoutSeconds));
            string url = $"{_gatewayConfig.ServiceDiscoveryProtocol}://{_gatewayConfig.ServiceDiscoveryHost}{ServiceDiscoveryKey.UnregisterPath}";
            using var res = await _httpClient.GetAsync(url, cts.Token);

            if (res.IsSuccessStatusCode)
            {
                _logger.LogInformation("The client has successfully logged out");
            }
            else
            {
                _logger.LogInformation("The client logout failed");
            }
        }
    }
}
