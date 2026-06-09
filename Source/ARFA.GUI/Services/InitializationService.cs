using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Prism.Events;
using RIVS.ASAK.ARFA.GUI.EventArgs;
using RIVS.ASAK.Core.Contract.DTO;
using RIVS.ASAK.Core.Contract;

namespace RIVS.ASAK.ARFA.GUI.Services
{
    public interface IInitializationService
    {
        Task<bool> LoadInitializationDataAsync();
        bool IsInitialized { get; }
        void Initialize();
    }

    public class InitializationService : IInitializationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IEventAggregator _eventAggregator;
        private readonly ISignalRService _signalRService;

        private string _baseUrl;

        public bool IsInitialized { get; private set; }

        public InitializationService(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            IEventAggregator eventAggregator,
            ISignalRService signalRService)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = loggerFactory.Create(GetType());
            _eventAggregator = eventAggregator;
            _signalRService = signalRService;

        }

        public void Initialize()
        {
            try
            {
                var ip = _configuration.GetSection("webclient:ip").Value;
                var port = _configuration.GetSection("webclient:port").Value;
                _baseUrl = $"http://{ip}:{port}/api/";
            }
            catch (Exception ex)
            {
                _logger.Error($"InitializationService Initialize {ex.Message}");
            }
        }

        public async Task<bool> LoadInitializationDataAsync()
        {
            try
            {
                // Шаг 1: Загружаем начальные данные через REST API (быстро и надежно)
                _logger.Info("Loading initial data via REST API...");
                await LoadDataViaRestApiAsync();

                // Шаг 2: Подключаемся к SignalR для real-time обновлений
                _logger.Info("Connecting to SignalR for real-time updates...");
                var signalRConnected = await _signalRService.ConnectAsync();

                if (signalRConnected)
                {
                    _logger.Info("SignalR connected successfully. Real-time updates enabled.");
                }
                else
                {
                    _logger.Warn("SignalR connection failed. Application will work without real-time updates.");
                }

                //TODO: поправить - вот так просто ставить IsInitialized в true не совсем правильно,
                //т.к. если SignalR не подключится, то real-time обновления работать не будут, а пользователь может этого не понять
                //и увязать с REST
                IsInitialized = true;
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("LoadInitializationDataAsync failed", ex);
                return false;
            }
        }

        private async Task LoadDataViaRestApiAsync()
        {
            var tasks = new Task[]
            {
                GetAppArfaStateAsync(CancellationToken.None),
                GetImportantMessagesAsync(CancellationToken.None),
                GetMessagesAsync(CancellationToken.None)
            };

            await Task.WhenAll(tasks);
        }

        private async Task<bool> GetAppArfaStateAsync(CancellationToken ct)
        {
            using (var client = _httpClientFactory.CreateClient())
            {
                try
                {
                    var response = await client.GetAsync($"{_baseUrl}app-state", ct);
                    var result = await response.Content
                        .ReadFromJsonAsync<ApplicationStateDto>(cancellationToken: ct);

                    //публикуем событие для UI для отображения устройств
                    if (result != null)
                    {
                        _eventAggregator.GetEvent<InitializeGuiRequestResult>().Publish(new InitializeGuiRequestResultArgs(true, result.State));
                        return true;
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    _logger.Error("InitializationService.GetAppCsuStateAsync", ex);
                    return false;
                }
            }
        }

        private async Task<IEnumerable<MessageDto>> GetImportantMessagesAsync(CancellationToken ct)
        {
            using (var client = _httpClientFactory.CreateClient())
            {
                try
                {
                    var response = await client.GetAsync($"{_baseUrl}message/important", ct);
                    var result = await response.Content
                        .ReadFromJsonAsync<IEnumerable<MessageDto>>(
                            new JsonSerializerOptions
                            {
                                Converters = { new JsonStringEnumConverter() }
                            },
                            cancellationToken: ct);

                    if (result != null)
                    {
                        _eventAggregator.GetEvent<ImportantMessagesUpdatedEvent>().Publish(result);
                    }

                    return result ?? Enumerable.Empty<MessageDto>();
                }
                catch (Exception ex)
                {
                    _logger.Error("InitializationService.GetImportantMessagesAsync", ex);
                    return null;
                }
            }
        }

        private async Task<IEnumerable<MessageDto>> GetMessagesAsync(CancellationToken ct)
        {
            using (var client = _httpClientFactory.CreateClient())
            {
                try
                {
                    var response = await client.GetAsync($"{_baseUrl}message/general", ct);
                    var result = await response.Content
                        .ReadFromJsonAsync<IEnumerable<MessageDto>>(
                            new JsonSerializerOptions
                            {
                                Converters = { new JsonStringEnumConverter() }
                            },
                            cancellationToken: ct);

                    if (result != null)
                    {
                        _eventAggregator.GetEvent<GeneralMessagesUpdatedEvent>().Publish(result);
                    }

                    return result ?? Enumerable.Empty<MessageDto>();
                }
                catch (Exception ex)
                {
                    _logger.Error("InitializationService.GetMessagesAsync", ex);
                    return null;
                }
            }
        }
    }
}
