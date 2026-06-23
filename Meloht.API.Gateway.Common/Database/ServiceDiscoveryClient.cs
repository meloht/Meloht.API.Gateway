using Meloht.API.Gateway.Common.Configuration;
using Meloht.API.Gateway.Common.HealthCheck;
using Meloht.API.Gateway.Common.Utilities;
using Meloht.API.Gateway.ServerProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Meloht.API.Gateway.Common.Database
{
    public class ServiceDiscoveryClient : ServerBase
    {
        private int _timeoutSeconds;
        private string _serviceHost;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ServiceDiscoveryClient> _logger;
        private readonly ObjectPool<StringBuilder> _poolStringBuilder;
        public ServiceDiscoveryClient(HealthCheckServer healthCheckServer, IOptionsMonitor<ServiceDiscoveryConfig> options, ILogger<ServiceDiscoveryClient> logger, IHttpClientFactory httpClientFactory)
            : base(healthCheckServer)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient(HttpClientKey.ClientKey);
            options.OnChange(OnConfigChange);
            OnConfigChange(options.CurrentValue);

            _poolStringBuilder = new ObjectPool<StringBuilder>(() => new StringBuilder(), maxSize: 10, resetAction: ResetStringBuilder);
        }
        private void ResetStringBuilder(StringBuilder sb)
        {
            sb.Clear();
        }
        private void OnConfigChange(ServiceDiscoveryConfig config)
        {
            if (config != null && config.RequestTimeoutSeconds > 0)
            {
                _timeoutSeconds = config.RequestTimeoutSeconds;
            }
            else
            {
                _timeoutSeconds = ServiceDiscoveryConfig.RequestTimeoutSecondsDefault;
            }

            if (config != null && string.IsNullOrWhiteSpace(config.ServiceDiscoveryHost))
            {
                _serviceHost = config.ServiceDiscoveryHost;
            }
            else
            {
                throw new ArgumentNullException("ServiceDiscoveryHost");
            }

        }

        public async Task DataReadAsync(ParallelOptions parallelOptions, CancellationToken cancellationToken)
        {
            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(_timeoutSeconds));
                string url = GetUrl();
                using var res = await _httpClient.GetAsync(url, cts.Token);
                if (res.IsSuccessStatusCode)
                {
                    var resData = await res.Content.ReadFromJsonAsync<ResponseMessage<List<ServerNodeConfig>>>();
                    if (resData != null)
                    {
                        if (resData.StatusCode == ResponseStatus.SuccessCode)
                        {
                            List<ServerNode> serverNodes = AppUtils.UpdateData(resData.Data, _serversDict);

                            UpdateOriginalList(serverNodes);
                            await _healthCheckServer.CheckServerHealthAsync(parallelOptions, serverNodes);
                            UpdateHealthlList(serverNodes);
                        }
                        else
                        {
                            _logger.LogError($"The responseMessage StatusCode is {resData.StatusCode}");
                        }

                    }
                    else
                    {
                        _logger.LogError($"The responseMessage is null");
                    }
                }
                else
                {
                    _logger.LogInformation($"The responseMessage is failed, status code: {res.StatusCode}");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error get client list of server {server}", _serviceHost);
            }

        }

        private string GetUrl()
        {
            var sb = _poolStringBuilder.Rent();
            try
            {
                sb.Append(_serviceHost);
                sb.Append(ServiceDiscoveryKey.GetClientsPath);
                return sb.ToString();
            }
            finally
            {
                _poolStringBuilder.Return(sb);
            }
        }

        public List<ServerNode> GetHealthyServers()
        {
            return _serversHealthList;
        }

        public List<ServerNode> GetAllServers()
        {
            return _serversOriginalList;
        }

        public int GetServerWeightSum()
        {
            return _weightSum;
        }
    }
}
