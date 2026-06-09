using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Prism.Events;
using RIVS.ASAK.ARFA.GUI.EventArgs;
using RIVS.ASAK.ARFA.GUI.Services;
using RIVS.ASAK.Core.Contract.DTO;
using RIVS.ASAK.Core.Contract.Enums;
using RIVS.ASAK.Core.Contract;
using RIVS.ASAK.UIElements.Tools;
using RIVS.ASAK.UIElements.Tools.LogMessage;

namespace RIVS.ASAK.ARFA.GUI.ViewModels
{
    public sealed class MainViewModel : BaseViewModel, IMainViewModel
    {
        private readonly ILogger _logger;
        private readonly IEventAggregator _eventAggregator;
        private readonly IArfaConfigurationResolver _arfaConfigurationResolver;

        //Максимальное количество важных сообщений на главном окне
        private readonly int _maxCapacity = 20;
        private CustomObservableCollection<ILogMessageDescription> _logMessage;
        public CustomObservableCollection<ILogMessageDescription> LogMessage => _logMessage ??= new CustomObservableCollection<ILogMessageDescription>();
        public ParametersViewModel ParametersViewModel { get; }
        public MeasurementResultsViewModel MeasurementResultsViewModel { get; }

        private CuvetteBlockViewModelBase _cuvetteViewModel;
        public CuvetteBlockViewModelBase CuvetteViewModel
        {
            get { return _cuvetteViewModel; }
            set
            {
                _cuvetteViewModel = value;
                OnPropertyChanged(nameof(CuvetteViewModel));
            }
        }

        //public event EventHandler<bool> DevFlagChanged;

        private bool _devFlag;
        public bool DevFlag
        {
            get => _devFlag;
            set
            {
                _devFlag = value;
                OnPropertyChanged(nameof(DevFlag));
                //DevFlagChanged?.Invoke(this, _devFlag);
                _eventAggregator.GetEvent<DevFlagChangedEvent>()
                    .Publish(new DevFlagChangedEventArgs(value));
            }
        }

        //public event EventHandler<bool> AuthCompletedFlagChanged;

        private bool _authCompletedFlag;
        public bool AuthCompletedFlag
        {
            get => _authCompletedFlag;
            set
            {
                _authCompletedFlag = value;
                OnPropertyChanged(nameof(AuthCompletedFlag));
                //AuthCompletedFlagChanged?.Invoke(this, _authCompletedFlag);
                _eventAggregator.GetEvent<AuthCompletedFlagChangedEvent>()
                    .Publish(new AuthCompletedFlagChangedEventArgs(value));
            }
        }

        private bool _isNeedToAuthAgain;
        public bool IsNeedToAuthAgain
        {
            get => _isNeedToAuthAgain;
            set
            {
                _isNeedToAuthAgain = value;
                OnPropertyChanged(nameof(IsNeedToAuthAgain));
            }
        }

        private bool _autoMode;
        public bool AutoMode
        {
            get => _autoMode;
            set
            {
                _autoMode = value;
                OnAutoModeChanged(_autoMode);
            }
        }
        
        public MainViewModel(IConfiguration configuration,
            //OperationViewModel operationViewModel,
            ILoggerFactory loggerFactory,
            ParametersViewModel parametersViewModel,
            MeasurementResultsViewModel measurementResultsViewModel,
            CuvetteBlockViewModelBase cuvetteBlockViewModel,
            IEventAggregator eventAggregator,
            IArfaConfigurationResolver arfaConfigurationResolver
            )
        {
            _logger = loggerFactory.Create(GetType());

            _eventAggregator = eventAggregator;
            
            eventAggregator.GetEvent<StartCommandOperationEvent>().Subscribe(OnStartCommandOperationEvent);
            eventAggregator.GetEvent<StopCommandOperationEvent>().Subscribe(OnStopCommandOperationEvent);
            eventAggregator.GetEvent<AutoModeCommandOperationEvent>().Subscribe(OnAutoModeCommandOperationEvent);

            ParametersViewModel = parametersViewModel;
            MeasurementResultsViewModel = measurementResultsViewModel;
            CuvetteViewModel = cuvetteBlockViewModel;

            _arfaConfigurationResolver = arfaConfigurationResolver;

            //TODO: реализовать подписки
            //eventAggregator.GetEvent<ImportantMessagesUpdatedEvent>().Subscribe(OnImportantMessagesUpdated, ThreadOption.BackgroundThread);
            //eventAggregator.GetEvent<ImportantMessagesReceivedEvent>().Subscribe(OnImportantMessagesReceived, ThreadOption.BackgroundThread);

        }

        #region LogMessage

        private void AddLogMessage(LogMessageDescription log)
        {
            _logMessage.Add(log);
            ResortMsgLists();
        }

        private void ResortMsgLists()
        {
            var delta = _logMessage.Count - _maxCapacity;
            _logMessage.Sort((f1, f2) => Math.Sign(f1.Time.Ticks - f2.Time.Ticks));
            if (delta > 0)
            {
                _logMessage.RemoveRange(_logMessage.Take(delta));
            }
        }

        #endregion LogMessage


        public async Task LoadAsync()
        {
            await Task.CompletedTask;
        }

        //// определяем нужную нам vm - 10/15 кювет
        //private CuvetteBlockViewModelBase SelectedCuvetteBlockViewModel(IConfiguration configuration)
        //{
        //    var strCuvetteCount = configuration.GetSection("appConfig:NumberOfCuvettes").Value;
        //    int.TryParse(strCuvetteCount, out var cuvetteCount);
        //    if (cuvetteCount == 15)
        //    {
        //        return AppSettings.Resolve<CuvetteBlock15ViewModel>();
        //    }
        //    else
        //    {
        //        return AppSettings.Resolve<CuvetteBlock10ViewModel>();
        //    }
        //}

        private void OnAutoModeChanged(bool mode)
        {
            if (DevFlag || !mode)
            {
                _cuvetteViewModel.IsButtonsEnabled = true;
            }
            else
            {
                _cuvetteViewModel.IsButtonsEnabled = false;
            }
            
        }

        /// <summary>
        /// Обработчик кнопки Start
        /// </summary>
        /// <param name="obj"></param>
        private async void OnStartCommandOperationEvent(StartCommandOperationEventArgs obj)
        {
            bool resultStart;
            //запрашиваем у диспетчера конфигурацию
            var taskInitialization = await Task.Run(IsReceivedInitializationData);

            if (taskInitialization)
            {
                GenerateMessageEvent("От Диспетчера получены данные для инициализации");

                // написать имя АП и время активации, подсветить кнопку АП
                var analyticProgramInfo = _arfaConfigurationResolver.GetAnalyticProgramInfo();
                _eventAggregator.GetEvent<AnalyticalProgramChangedEvent>().Publish(
                    new AnalyticalProgramChangedEventArgs(true, analyticProgramInfo.Item1, analyticProgramInfo.Item2));

                // подсветить кюветы
                var highlightCuvetteLst = _arfaConfigurationResolver.GetUsedCuvettesFromAnalyticProgram();
                _eventAggregator.GetEvent<HighlightCuvetteChangedEvent>().Publish(
                    new HighlightCuvetteChangedEventArgs(highlightCuvetteLst.ToList()));
            }
            else
            {
                //TODO добавить сообщение в лог и в интерфейс
                //_dispatcherAction.Invoke(() =>
                //    RaiseMessageEvent(EventCategoryType.Error, "От Диспетчера не получены данные для инициализации"));
            }

            //стартуем независимо от того, если АП или нет
            resultStart = await StartOperationAsync();

            _eventAggregator.GetEvent<StartCommandOperationResultEvent>().Publish(new StartCommandOperationResultEventArgs(resultStart));

        }

        /// <summary>
        /// Обработчик нажатия кнопки Start/Stop - перевод в состояние STOP
        /// </summary>
        /// <param name="obj"></param>
        private async void OnStopCommandOperationEvent(StopCommandOperationEventArgs obj)
        {
            //отключение устройств
            bool resultStop;
            
            //TODO: отпрвили команду стоп? где?
            //var confirmationData = false;
            //if (resultStopDevices is ErrorResult)
            //{
            //    //сообщаем об ошибке
            //    //resultStop = false;
            //}
            //if (resultStopDevices is bool boolean)
            //{
            //    confirmationData = boolean;
            //}
            //
            //resultStop = confirmationData;

            // снимаем подсветку с кювет
            _eventAggregator.GetEvent<HighlightCuvetteChangedEvent>().Publish(
                new HighlightCuvetteChangedEventArgs(new List<int>()));

            // убираем название АП и подсветку кнопки
            _eventAggregator.GetEvent<AnalyticalProgramChangedEvent>().Publish(
                new AnalyticalProgramChangedEventArgs());

            //TODO: по-идеи не тут. Кто отправил, тот и получил результат
            //_eventAggregator.GetEvent<StopCommandOperationResultEvent>().Publish(new StopCommandOperationResultEventArgs(resultStop));

        }

        Task<bool> StartOperationAsync()
        {
            var task = new Task<bool>(StartOperation);
            task.Start();
            return task;
        }

        private bool StartOperation()
        {
            //TODO: тут запускаемся
            //инициализируем сервис SpectrBlockDeviceManagerService
            //var spectrBlockDeviceManagerConfiguration = configurationResolver.GetSpectrBlockDeviceManagerConfiguration();
            //var servSpectrBlock = DestinationService.CreateLocalServiceDescription(SpectrBlockDeviceManagerService.ServiceId);
            //var cmdSpectrBlock = new Command(GeneralServiceCommands.UpdateEntityCollection, spectrBlockDeviceManagerConfiguration);
            //var resultSpectrBlock = AppSettings.Resolve<IBrokerWrapper>()
            //    .ExecuteCommandWithResponseSync(servSpectrBlock, cmdSpectrBlock, System.Threading.Timeout.Infinite);

            //var commandSpectrBlockResponse = resultSpectrBlock as CommandConfirmation;
            //if (commandSpectrBlockResponse is { Data: ErrorResult errorSpectrBlockResult })
            //{
            //    GenerateMessageEvent("Ошибка инициализации спектрометрического блока");
            //    _logger.Error("MainViewModel StartOperation: cmd: UpdateEntityCollection to SpectrBlockDeviceManagerService ErrorResult"
            //        , errorSpectrBlockResult.InnerException);
            //    return false;
            //}

            //инициализируем сервис ArfaDeviceManagerService
            //var arfaDeviceManagerConfiguration = configurationResolver.GetDeviceManagerConfiguration();
            //var servArfaDeviceManager = DestinationService.CreateLocalServiceDescription(ArfaDeviceManagerService.ServiceId);
            //var cmdArfaDeviceManager = new Command(GeneralServiceCommands.UpdateEntityCollection, arfaDeviceManagerConfiguration);
            //var resultArfaDevMngr = AppSettings.Resolve<IBrokerWrapper>()
            //    .ExecuteCommandWithResponseSync(servArfaDeviceManager, cmdArfaDeviceManager, System.Threading.Timeout.Infinite);

            //var commandArfaDevMngrResponse = resultArfaDevMngr as CommandConfirmation;
            //if (commandArfaDevMngrResponse is { Data: ErrorResult errorArfaDevMngrResult })
            //{
            //    _logger.Error("MainViewModel StartOperation: cmd: UpdateEntityCollection to ArfaDeviceManagerService ErrorResult"
            //        , errorArfaDevMngrResult.InnerException);
            //    return false;
            //}

            // стартуем устройства
            GenerateMessageEvent("Старт спектрометрического блока");
            //получаем канал для отправки в него статусов - его надо передать как можно глубже, вплоть до драйвера
            //var tubeStateChannel = AppSettings.Resolve<IParametersViewModel>().GetTubeStateChannels();
            //var detectorStateChannel = AppSettings.Resolve<IParametersViewModel>().GetDetectorStateChannels();
            //var motorStateChannel = AppSettings.Resolve<IParametersViewModel>().GetMotorStateChannels();
            //var resultResponse = AppSettings.Resolve<IBrokerWrapper>()
            //    .ExecuteCommandWithResponseSync(
            //        DestinationService.CreateLocalServiceDescription(ArfaExecutiveManagerService.ServiceId),
            //        new Command(ArfaExecutiveManagerService.StartDevices)
            //        {
            //            CommandData = new DeviceStateCommandParams(tubeStateChannel, detectorStateChannel, motorStateChannel)
            //        }, System.Threading.Timeout.Infinite);
            //var confirmationData = false;
            //if (resultResponse is ErrorResult result)
            //{
            //    GenerateMessageEvent("Ошибка старта спектрометрического блока", EMessageType.Error);
            //    var ex = result.InnerException;
            //    _logger.Error("MainViewModel StartOperation: cmd: StartDevices to ArfaExecutiveManagerService ErrorResult"
            //        , ex);
            //    return false;
            //}
            //if (resultResponse is bool boolean)
            //{
            //    confirmationData = boolean;
            //}

            //return confirmationData;
            return true;

        }

        /// <summary>
        /// Проверяем, получены ли инициализационные данные
        /// </summary>
        public bool IsReceivedInitializationData()
        {
            //TODO: что-то про инициализацию
            //if (_isInitializationSuccess)
            //{
            //    return true;
            //}

//            var attemptCount = 0;
//            var allowDoWork = false;
//            var timeout = new TimeSpan(0, 0, 10);
//            var sync = new AutoResetEvent(false);
//#pragma warning disable IDE0039 // Использовать локальную функцию
//            Action afterInitSuccessfullyCallbackHandler = () =>
//#pragma warning restore IDE0039 // Использовать локальную функцию
//            {
//                allowDoWork = true;
//                sync.Set();
//            };
//            do
//            {
//                System.Windows.Application.Current.Dispatcher.Invoke(
//                    DispatcherPriority.Normal,
//                    (ThreadStart)delegate
//                    {
//                        try
//                        {
//                            _arfaConfigurationService.GetInitData(afterInitSuccessfullyCallbackHandler);
//                            Trace.TraceInformation($"{DateTime.Now:HH-mm-ss-fff} вызван GetInitData");
//                        }
//                        catch (Exception)
//                        {
//                            Trace.TraceError($"{DateTime.Now:HH-mm-ss-fff} Не смогли получить инициализационные данные");
//                            //RaiseMessageEvent(EventCategoryType.Error,
//                            //    $"Не смогли получить инициализационные данные. Пробуем еще раз");
//                            Thread.Sleep(100);
//                        }
//                    });
//                attemptCount++;
//                Trace.TraceInformation($"Ожидаем инициализации");
//                sync.WaitOne(timeout);
//                Trace.TraceInformation($"{DateTime.Now:HH-mm-ss-fff} После ожидания: allowDoWork={allowDoWork}, attemptCount={attemptCount}");
//            } while (!allowDoWork && attemptCount < 2);

//            return allowDoWork;
            return true;
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Автоматический режим" из UI
        /// </summary>
        /// <param name="args"></param>
        private async void OnAutoModeCommandOperationEvent(AutoModeCommandOperationEventArgs args)
        {
            //фактичеки по команде меняется значение Auto в OPC
            // собственно туда и отправляем команду
            var isAutoMode = args.Cmd == EAutoModeCmd.On;
            //TODO: кто отправляет? Кто публикует результат?
            //var resultSetAutoMode = await Task.Run(() => AppSettings.Resolve<IBrokerWrapper>().
            //    ExecuteCommandWithResponseSync(
            //        DestinationService.CreateLocalServiceDescription(OpcServerService.ServiceId),
            //        new Command(OpcServerService.SetAutoModeCommand)
            //            { CommandData = new AutoModeCommandParams(isAutoMode) })
            //);

            //var confirmationData = false;
            //if (resultSetAutoMode is bool boolean)
            //{
            //    confirmationData = boolean;
            //}

            //_eventAggregator
            //    .GetEvent<AutoModeCommandOperationResultEvent>()
            //    .Publish(new AutoModeCommandOperationResultEventArgs(args.Cmd, confirmationData, resultSetAutoMode is ErrorResult));

        }

        private void GenerateMessageEvent(string message, EMessageType messageType = EMessageType.Debug)
        {
            var eventId = EventTypes.CsuLogMessage;
            var msg = new LogMessage(messageType, message);
            object extData = msg;
            var action = new Action(() =>
            {
                // попробуем клонировать extData, иначе, объект может быть изменен позже, до того как событие достигнет подписчика.
                var newExtData = extData is ICloneable cloneableData ? cloneableData.Clone() : extData;
                //var newEvent = new Core.CoreInterfaces.Blackboard.Event(eventId, Guid.Empty, Guid.Empty, newExtData, null, false);
                //AppServiceLocator.Resolve<IBlackboardWrapper>().PublishEvent(newEvent);
            });
            var task = new Task(action);
            task.Start();
        }


    }
}
