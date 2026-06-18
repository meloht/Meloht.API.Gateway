using Meloht.API.Gateway.Common;
using Meloht.API.Gateway.Common.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
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

            var serverNode = new ServerNodeConfig(_gatewayConfig.ClientProtocol, _gatewayConfig.ClientWeight, _gatewayConfig.ClientUniqueName);

            using var res = await _httpClient.PostAsJsonAsync<ServerNodeConfig>(url, serverNode, cts.Token);

            await CheckResultAsync(res, "The client has registered successfully", "The client has registered failed");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(_gatewayConfig.RequestTimeoutSeconds));
            string url = $"{_gatewayConfig.ServiceDiscoveryProtocol}://{_gatewayConfig.ServiceDiscoveryHost}{ServiceDiscoveryKey.UnregisterPath}";
            using var res = await _httpClient.GetAsync(url, cts.Token);

            await CheckResultAsync(res, "The client has successfully logged out", "The client logout failed");
        }

        private async Task CheckResultAsync(HttpResponseMessage res, string successMsg, string failedMsg)
        {
            if (res.IsSuccessStatusCode)
            {
                var resData = await res.Content.ReadFromJsonAsync<ResponseMessage>();
                if (resData != null)
                {
                    if (resData.Status == ResponseStatus.Success)
                    {
                        _logger.LogInformation(successMsg);
                    }
                    else
                    {
                        _logger.LogError(failedMsg);
                        throw new Exception(failedMsg);
                    }
                }
                else
                {
                    _logger.LogError("The responseMessage is null");
                    throw new Exception("The responseMessage is null");
                }
            }
            else
            {
                _logger.LogInformation($"The responseMessage is failed, status code: {res.StatusCode}");
                throw new Exception($"The responseMessage is failed, status code: {res.StatusCode}");
            }
        }
    }
}
