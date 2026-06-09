using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Prism.Events;
using RIVS.ASAK.ARFA.GUI.EventArgs;
using RIVS.ASAK.Core.Contract;
using RIVS.ASAK.Core.Contract.DTO;

namespace RIVS.ASAK.ARFA.GUI.Services
{
    public interface ISignalRService
    {
        bool IsConnected { get; }
        Task<bool> ConnectAsync();
        Task DisconnectAsync();
        //void Reconnect(); //к удалению!
    }

    
    /// <summary>
    /// Сервис для управления SignalR соединением и публикации полученных данных через Prism Events.
    /// <remarks>
    /// SignalRService не реконнектится сам.
    /// SignalR просто клиент: ConnectAsync / DisconnectAsync 
    /// </remarks>
    /// </summary>
    public class SignalRService : ISignalRService
    {
        private readonly HubConnection _connection;
        private readonly IHubProxy _proxy;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger _logger;
        private bool _isConnected;

        public bool IsConnected => _isConnected;

        public SignalRService(
          IEventAggregator eventAggregator,
          ILoggerFactory loggerFactory,
          string serverUrl = "http://localhost:10003")
        {
            _eventAggregator = eventAggregator;
            _logger = loggerFactory.Create(GetType());
            _connection = new HubConnection(serverUrl);
            _proxy = _connection.CreateHubProxy("frontendHub");

            SetupConnectionHandlers();
            SetupDataHandlers();
        }

        private void SetupConnectionHandlers()
        {
            _connection.StateChanged += change =>
            {
                _isConnected = change.NewState == ConnectionState.Connected;
                _logger.Info($"SignalR connection state changed: {change.OldState} -> {change.NewState}");
                _eventAggregator.GetEvent<ConnectionStatusChangedEvent>().Publish(_isConnected);
            };

            _connection.Closed += () =>
            {
                _logger.Warn("SignalR connection closed.");
                //автореконнект из SignalRService убран, потому что теперь этим занимается ConnectionMonitorService!
            };

            _connection.Error += ex =>
            {
                _logger.Error("SignalR connection error", ex);
            };
        }

        private void SetupDataHandlers()
        {
            

            // Получение одного сообщения
            _proxy.On<MessageDto>("ReceiveMessage", message =>
            {
                _logger.Debug($"Received Message: {message?.Text}");
                _eventAggregator.GetEvent<MessageReceivedEvent>().Publish(message);
            });

            // Получение списка сообщений
            _proxy.On<IEnumerable<MessageDto>>("ReceiveMessages", messages =>
            {
                _logger.Debug("Received Messages collection");
                _eventAggregator.GetEvent<MessagesReceivedEvent>().Publish(messages);
            });

            // Получение важных сообщений
            _proxy.On<IEnumerable<MessageDto>>("ReceiveImportantMessages", messages =>
            {
                _logger.Debug("Received ImportantMessages collection");
                _eventAggregator.GetEvent<ImportantMessagesReceivedEvent>().Publish(messages);
            });

            _proxy.On<ApplicationStateDto>("ReceiveApplicationState", state =>
            {
                _eventAggregator.GetEvent<ApplicationStateReceivedEvent>().Publish(state);
            });
        }

        public async Task<bool> ConnectAsync()
        {
            if (_connection.State == ConnectionState.Connected)
            {
                _logger.Info("SignalR already connected");
                return true;
            }

            if (_connection.State == ConnectionState.Connecting)
            {
                _logger.Info("SignalR connection in progress");
                return false;
            }

            try
            {
                _logger.Info("Connecting to SignalR...");
                await _connection.Start();
                _logger.Info($"SignalR connected successfully. ConnectionId: {_connection.ConnectionId}");
                return _connection.State == ConnectionState.Connected;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to connect to SignalR", ex);
                return false;
            }
        }

        public async Task DisconnectAsync()
        {
            if (_connection.State == ConnectionState.Connected ||
                _connection.State == ConnectionState.Reconnecting)
            {
                _logger.Info("Disconnecting from SignalR...");
                _connection.Stop();
                await Task.CompletedTask;
            }
        }
    }

}