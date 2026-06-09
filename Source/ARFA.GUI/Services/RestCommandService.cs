using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Prism.Events;
using RIVS.ASAK.Core.Contract;
using System.Text.Json;

namespace RIVS.ASAK.ARFA.GUI.Services
{
    public interface IRestCommandService
    {
        Task<bool> StartArfaAsync(CancellationToken ct = default);
        Task<bool> StopArfaAsync(CancellationToken ct = default);
    }

    public class RestCommandService : IRestCommandService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;
        private readonly IEventAggregator _eventAggregator;

        private readonly string _baseUrl;

        public RestCommandService(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            IEventAggregator eventAggregator)
        {
            _httpClientFactory = httpClientFactory;
            _logger = loggerFactory.Create(GetType());
            _eventAggregator = eventAggregator;

            var ip = configuration.GetSection("webclient:ip").Value;
            var port = configuration.GetSection("webclient:port").Value;
            _baseUrl = $"http://{ip}:{port}/api/";
        }
        

        public Task<bool> StartArfaAsync(CancellationToken ct = default)
            => SendArfaStateCommandAsync(true, ct);

        public Task<bool> StopArfaAsync(CancellationToken ct = default)
            => SendArfaStateCommandAsync(false, ct);

        private async Task<bool> SendArfaStateCommandAsync(bool state, CancellationToken ct)
        {
            var body = JsonSerializer.Serialize(state);
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            using (var client = _httpClientFactory.CreateClient())
            {
                try
                {
                    var response = await client.PostAsync($"{_baseUrl}app-state", content, ct);
                    response.EnsureSuccessStatusCode();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Error("RestCommandService.SendArfaStateCommandAsync", ex);
                    return false;
                }
            }
        }
    }
}
