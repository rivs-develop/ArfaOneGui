using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Prism.Events;
using RIVS.ASAK.Core.Contract;
using System.Net.Http;
using RIVS.ASAK.ARFA.GUI.EventArgs;

namespace RIVS.ASAK.ARFA.GUI.Services
{
    public enum BackendConnectionState
    {
        Unknown,
        Connected,
        Disconnected
    }

    public interface IConnectionMonitorService
    {
        BackendConnectionState CurrentState { get; }
        void Start();
        void Stop();
    }

    /// <summary>
    /// Следит за доступностью бекенда через периодические запросы к /api/health.
    /// При 3 подряд неудачах — объявляет дисконнект, отключает SignalR.
    /// При восстановлении — перезагружает данные по REST и переподключает SignalR.
    /// </summary>
    public class ConnectionMonitorService : IConnectionMonitorService, IDisposable
    {
        // Константы для настройки поведения мониторинга
        private const int FailThreshold = 3;          // сколько подряд фейлов = дисконнект
        private const int NormalIntervalMs = 7_000;      // интервал когда всё ок
        private const int WaitingIntervalMs = 15_000;     // интервал когда ждём восстановления
        private const int RequestTimeoutMs = 3_000;      // таймаут одного /health запроса

        // Внедряем IHttpClientFactory для пингов,
        // IInitializationService для восстановления,
        // ISignalRService для отключения при дисконнекте,
        // IEventAggregator для публикации статуса,
        // ILogger для логов.
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IInitializationService _initializationService;
        private readonly ISignalRService _signalRService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger _logger;
        private readonly string _healthUrl;


        private CancellationTokenSource _cts;
        private int _consecutiveFails = 0;
        private bool _running = false;

        public BackendConnectionState CurrentState { get; private set; } = BackendConnectionState.Unknown;

        public ConnectionMonitorService(
            IHttpClientFactory httpClientFactory,
            IInitializationService initializationService,
            ISignalRService signalRService,
            IEventAggregator eventAggregator,
            ILoggerFactory loggerFactory,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _initializationService = initializationService;
            _signalRService = signalRService;
            _eventAggregator = eventAggregator;
            _logger = loggerFactory.Create(GetType());

            var ip = configuration.GetSection("webclient:ip").Value;
            var port = configuration.GetSection("webclient:port").Value;
            _healthUrl = $"http://{ip}:{port}/api/health";
        }


        // Start запускает фоновый цикл мониторинга. Stop останавливает его. Dispose чистит ресурсы.
        public void Start()
        {
            if (_running)
            {
                return;
            }

            _running = true;
            _cts = new CancellationTokenSource();
            Task.Run(() => MonitorLoopAsync(_cts.Token));
            _logger.Info("ConnectionMonitor started");
        }

        public void Stop()
        {
            _running = false;
            _cts?.Cancel();
            _logger.Info("ConnectionMonitor stopped");
        }


        // В бесконечном цикле пингуем сервер. При каждом результате вызываем соответствующий обработчик.
        private async Task MonitorLoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var healthy = await PingAsync(ct);

                if (healthy)
                {
                    await OnSuccessAsync(ct);
                }
                else
                {
                    await OnFailureAsync(ct);
                }

                // адаптивный интервал: реже пинговать когда уже знаем что сервер упал
                var delay = CurrentState == BackendConnectionState.Disconnected
                    ? WaitingIntervalMs
                    : NormalIntervalMs;

                await Task.Delay(delay, ct).ContinueWith(_ => { }); // глотаем OperationCanceledException от Delay
            }
        }


        // При успешном пинге сбрасываем счётчик фейлов. Если были в Disconnected — пробуем восстановиться.
        private async Task OnSuccessAsync(CancellationToken ct)
        {
            _consecutiveFails = 0;

            if (CurrentState == BackendConnectionState.Disconnected)
            {
                // восстановление после подтверждённого дисконнекта
                _logger.Info("Backend is reachable. Restoring connection...");
                await RestoreAsync(ct);
            }
            else if (CurrentState == BackendConnectionState.Unknown)
            {
                // первый успешный пинг — просто фиксируем Connected,
                // данные загружает MainViewModel по InitializeGuiRequest
                CurrentState = BackendConnectionState.Connected;
                PublishState(connected: true);
            }
        }

        // При неудачном пинге увеличиваем счётчик фейлов. Если достигли порога — объявляем дисконнект.
        private async Task OnFailureAsync(CancellationToken ct)
        {
            _consecutiveFails++;
            _logger.Warn($"Health check failed ({_consecutiveFails}/{FailThreshold})");

            if (_consecutiveFails >= FailThreshold && CurrentState != BackendConnectionState.Disconnected)
            {
                _logger.Error("Backend is unreachable. Declaring disconnect.");
                await DisconnectAsync();
            }
        }


        // Переход в состояние Disconnected: обновляем статус, отключаем SignalR, публикуем событие.
        private async Task DisconnectAsync()
        {
            CurrentState = BackendConnectionState.Disconnected;

            // Отключаем SignalR — он всё равно уже мёртв или скоро умрёт
            try
            {
                await _signalRService.DisconnectAsync();
                _logger.Info("SignalR disconnected by ConnectionMonitor");
            }
            catch (Exception ex)
            {
                _logger.Error("Error disconnecting SignalR", ex);
            }

            PublishState(connected: false);
        }


        // При восстановлении перезагружаем данные по REST (это также переподключает SignalR внутри InitializationService),
        private async Task RestoreAsync(CancellationToken ct)
        {
            try
            {
                // 1. Перезагружаем данные по REST (это также переподключает SignalR внутри)
                var ok = await _initializationService.LoadInitializationDataAsync();

                if (ok)
                {
                    CurrentState = BackendConnectionState.Connected;
                    _consecutiveFails = 0;
                    _logger.Info("Connection restored: REST data loaded, SignalR connected");
                    PublishState(connected: true);
                }
                else
                {
                    // LoadInitializationDataAsync вернул false — трактуем как ещё один фейл
                    _logger.Warn("RestoreAsync: LoadInitializationDataAsync returned false");
                    _consecutiveFails++;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("RestoreAsync failed", ex);
                _consecutiveFails++;
            }
        }


        // Пингуем /api/health с таймаутом.
        // Если запрос отменён через CancellationToken — считаем что сервер жив, просто мы перестали ждать.
        private async Task<bool> PingAsync(CancellationToken ct)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromMilliseconds(RequestTimeoutMs);

                var response = await client.GetAsync(_healthUrl, ct);
                return response.IsSuccessStatusCode;
            }
            catch (OperationCanceledException)
            {
                // штатная отмена через CancellationToken — не считаем фейлом
                return true;
            }
            catch (Exception ex)
            {
                _logger.Debug($"Health check exception: {ex.Message}");
                return false;
            }
        }


        // Публикуем событие о смене статуса соединения с удобным текстом для отображения в UI.
        private void PublishState(bool connected)
        {
            _eventAggregator
                .GetEvent<BackendConnectionStatusChangedEvent>()
                .Publish(new BackendConnectionStatusChangedArgs(
                    connected,
                    connected ? "Соединение с сервером восстановлено" : "Сервер недоступен"));
        }


        // При Dispose останавливаем мониторинг и чистим ресурсы.
        public void Dispose()
        {
            Stop();
            _cts?.Dispose();
        }
    }
}
