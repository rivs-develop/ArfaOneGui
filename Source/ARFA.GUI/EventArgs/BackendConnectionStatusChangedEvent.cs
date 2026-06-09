using Prism.Events;

namespace RIVS.ASAK.ARFA.GUI.EventArgs
{
    /// <summary>
    /// Аргументы события изменения состояния соединения с бекендом.
    /// Агрегирует и REST и SignalR — подписчикам не нужно знать детали.
    /// </summary>
    public class BackendConnectionStatusChangedArgs
    {
        public bool IsConnected { get; }
        public string Message { get; }

        public BackendConnectionStatusChangedArgs(bool isConnected, string message = null)
        {
            IsConnected = isConnected;
            Message = message ?? (isConnected ? "Подключено" : "Отключено");
        }
    }

    /// <summary>
    /// Публикуется когда меняется общий статус доступности бекенда.
    /// Источник — ConnectionMonitorService (REST health-check).
    /// Не зависит от SignalR: бекенд может зависнуть при живом TCP-соединении.
    /// </summary>
    public class BackendConnectionStatusChangedEvent
        : PubSubEvent<BackendConnectionStatusChangedArgs>
    { }
}
