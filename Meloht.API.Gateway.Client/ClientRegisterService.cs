using Meloht.API.Gateway.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Client
{
    public class ClientRegisterService : IHostedService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _serviceDiscoveryHost;
        private readonly int? _port;
        private readonly string? _registerPath;
        private readonly string? _logoutPath;
        private readonly ILogger<ClientRegisterService> _logger;
        private readonly int _requestTimeoutSeconds;

        public ClientRegisterService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ClientRegisterService> logger)
        {
            _httpClient = httpClientFactory.CreateClient(HttpClientKey.ClientKey);
            _serviceDiscoveryHost = configuration.GetValue<string>(ServiceDiscoveryKey.ServiceDiscoveryHost);
            _registerPath = configuration.GetValue<string>(ServiceDiscoveryKey.RegisterPath);
            _logoutPath = configuration.GetValue<string>(ServiceDiscoveryKey.LogoutPath);
            _port = configuration.GetValue<int>(ServiceDiscoveryKey.ServiceDiscoveryPort);
            _requestTimeoutSeconds = configuration.GetValue<int>(ServiceDiscoveryKey.RequestTimeoutSeconds);
            _logger = logger;

        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_serviceDiscoveryHost))
            {
                _logger.LogError("service discovery host is null");
                throw new ArgumentException("service discovery host is null");
            }
            if (_port == null)
            {
                _logger.LogError("service discovery port is null");
                throw new ArgumentException("service discovery port is null");
            }
            if (string.IsNullOrEmpty(_registerPath))
            {
                _logger.LogError("service discovery register path is null");
                throw new ArgumentException("service discovery register path is null");
            }
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(_requestTimeoutSeconds));
            string url = $"http://{_serviceDiscoveryHost}:{_port.Value}/{_registerPath}";
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
            cts.CancelAfter(TimeSpan.FromSeconds(_requestTimeoutSeconds));
            string url = $"http://{_serviceDiscoveryHost}:{_port.Value}/{_logoutPath}";
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
