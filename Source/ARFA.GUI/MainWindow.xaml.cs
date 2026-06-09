using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Prism.Events;
using RIVS.ASAK.ARFA.GUI.EventArgs;
using RIVS.ASAK.ARFA.GUI.Helpers;
using RIVS.ASAK.ARFA.GUI.View;
using RIVS.ASAK.ARFA.GUI.ViewModels;
using RIVS.ASAK.Core.AuthorizationService;
using RIVS.ASAK.Core.Contract;
using RIVS.ASAK.Core.Contract.Configuration;
using RIVS.ASAK.UIElements.Authentication;
using RIVS.ASAK.UIElements.Controls.DateTimeControl;

namespace RIVS.ASAK.ARFA.GUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    [TemplatePart(Name = "PART_DateTimeControl", Type = typeof(DateTimeControl))]
    public partial class MainWindow : Window
    {
        private ConfigView _configPage;
        private readonly ConfigViewModel _configViewModel;
        /// <summary>
        /// Идентификатор подписки на события изменения режима работы
        /// </summary>
        //private Guid _autoModeEventsSubscriptionCookie = Guid.Empty;
        //private IReceiverChannel _eventChannel;
        private readonly IAuthorizationService _authorizationService;
        public IEventAggregator _eventAggregator;
        private readonly IAppConfigurationService _appConfigurationService;

        //таймер для делты между первой авторизацией
        private readonly DispatcherTimer _timer;
        private const int _authDeltaSeconds = 30;

        private MainViewModel _vm;

        private UIElements.Loading.SplashScreen _splashScreen;



        public MainWindow(IEventAggregator eventAggregator,
            IAuthorizationService authorizationService,
            ConfigViewModel configViewModel,
            IAppConfigurationService configurationService,
            MainViewModel vm)
        {
            InitializeComponent();
            
            _eventAggregator = eventAggregator;
            _authorizationService = authorizationService;
            _configViewModel = configViewModel;
            _appConfigurationService = configurationService;

            _vm = vm;

            _timer = new DispatcherTimer
            {
                // Устанавливаем интервал
                Interval = TimeSpan.FromSeconds(_authDeltaSeconds)
            };

            // Подписываемся на событие срабатывания таймера
            _timer.Tick += Timer_Tick; 

            Loaded += MainWindowLoaded;
            Unloaded += MainWindowUnloaded;

            DispatcherHelper.InterfaceLevelDispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
        }

        
        private void MainWindowLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            DataContext = _vm;
            _vm.LogMessage.CollectionChanged += LogMessage_CollectionChanged;
            //vm.eventAggregator.GetEvent<OperationEvent>().Subscribe(OnOperation);

            _eventAggregator.GetEvent<StartCommandOperationResultEvent>().Subscribe(OnStartCommandOperationResult);
            _eventAggregator.GetEvent<StopCommandOperationResultEvent>().Subscribe(OnStopCommandOperationResult);
            //eventAggregator.GetEvent<AutoModeCommandOperationResultEvent>().Subscribe(AutoModeCommandOperationResult);


            //подписка на событие успешной аутентификации
            _eventAggregator.GetEvent<AuthCompletedFlagChangedEvent>()
                .Subscribe(_configViewModel.OnAuthCompletedFlagChanged);
            //подписка на событие аутентификации роли разработчика
            _eventAggregator.GetEvent<DevFlagChangedEvent>()
                .Subscribe(_configViewModel.OnDevFlagChanged);

            //TODO:переделать подписки
            // Подписываемся на оповещения об изменении режима работы авт/ручной
            //var filter = new EventFilter();
            //filter.Types.Add(EventTypes.OPCAutoModeChanges);
            //AppSettings.Resolve<IBlackboardWrapper>()
            //    .Subscribe(filter, out _eventChannel, OnAutoModeChangedEventHandler, out _autoModeEventsSubscriptionCookie);

            //подписка на событие запроса повторной попытки аутентификации
            _eventAggregator.GetEvent<OpenAuthWindowRequestEvent>().Subscribe(OnOpenAuthWindowRequest);
            //подписка на событие запроса закрытия окна настроек
            _eventAggregator.GetEvent<CloseConfigWindowRequestEvent>().Subscribe(OnCloseConfigWindowRequest);

            PerformFirstAuthentication();
            
        }
        
        //логика переключения флага после срабатывания таймера
        private void Timer_Tick(object sender, System.EventArgs e)
        {
            // Останавливаем таймер
            _timer.Stop();

            // Переключаем флаг
            _vm.IsNeedToAuthAgain = true;
        }

        private void LogMessage_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action ==
                System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                if (VisualTreeHelper.GetChildrenCount(LogMessageList) > 0)
                {
                    FrameworkElement frameworkElement = (FrameworkElement)VisualTreeHelper.GetChild(LogMessageList, 0);
                    ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(frameworkElement, 0);
                    scrollViewer.ScrollToBottom();
                }
            }
        }

        private void MainWindowUnloaded(object sender, RoutedEventArgs e)
        {
            // Отписываемся от оповещения об изменении режима работы авт/ручной
            //if (Guid.Empty != _autoModeEventsSubscriptionCookie)
            //{
            //    AppSettings.Resolve<IBlackboardWrapper>().Unsubscribe(_autoModeEventsSubscriptionCookie);
            //    _autoModeEventsSubscriptionCookie = Guid.Empty;
            //}
        }

        //private void OnAutoModeChangedEventHandler(IReceiverChannel channel)
        //{
        //    try
        //    {
        //        channel.GetData(out var obj);
        //        var ev = (Event)obj;

        //        if (ev.ExtData is bool eventData)
        //        {
        //            if (eventData)
        //            {
        //                Dispatcher.BeginInvoke(new Action(() => 
        //                    AutoModeToggleButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF469646")));
        //            }
        //            else
        //            {
        //                Dispatcher.BeginInvoke(new Action(() =>
        //                    AutoModeToggleButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF2D2D30")));
        //            }
        //        }
        //        else
        //        {
        //            Debug.Assert(false, "Данные должны приходить типа {bool}");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        //Log.Error("Ошибка при обработке данных", e);
        //    }
        //}


        private void OnStartCommandOperationResult(StartCommandOperationResultEventArgs args)
        {
            if (args.CmdResult)
            {
                StartStopToggleButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF469646");
            }
            else
            {
                StartStopToggleButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFF3030");
            }
            CloseSplashScreen();
        }

        private void OnStopCommandOperationResult(StopCommandOperationResultEventArgs args)
        {
            if (args.CmdResult)
            {
                StartStopToggleButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF2D2D30");
            }
            else
            {
                StartStopToggleButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFF3030");
            }
            CloseSplashScreen();
        }


        private void CheckButtonStart(object sender, RoutedEventArgs e)
        {
            ShowSplashScreen("Инициализация...", 30);

            if (_vm.AuthCompletedFlag && (!_vm.IsNeedToAuthAgain || _vm.DevFlag))
            {
                _eventAggregator.GetEvent<StartCommandOperationEvent>().Publish(new StartCommandOperationEventArgs());
                return;
            }

            if (AuthenticationConfirmation(Permissions.SystemOnOff))
            {
                _eventAggregator.GetEvent<StartCommandOperationEvent>().Publish(new StartCommandOperationEventArgs());
            }
             
        }

        private void UncheckButtonStart(object sender, RoutedEventArgs e)
        {
            ShowSplashScreen("Остановка работы...", 30);
            if (_vm.AuthCompletedFlag && (!_vm.IsNeedToAuthAgain || _vm.DevFlag))
            {
                _eventAggregator.GetEvent<StopCommandOperationEvent>().Publish(new StopCommandOperationEventArgs());
                return;
            }
            if (AuthenticationConfirmation(Permissions.SystemOnOff))
            {
                _eventAggregator.GetEvent<StopCommandOperationEvent>().Publish(new StopCommandOperationEventArgs());
            }
        }

        private void CheckButtonAutoMode(object sender, RoutedEventArgs e)
        {
            if (_vm.AuthCompletedFlag && (!_vm.IsNeedToAuthAgain || _vm.DevFlag))
            {
                _eventAggregator.GetEvent<AutoModeCommandOperationEvent>()
                    .Publish(new AutoModeCommandOperationEventArgs(EAutoModeCmd.On));
                _vm.AutoMode = true;
                return;
            }

            if (AuthenticationConfirmation(Permissions.SwitchModeManualAuto))
            {
                _eventAggregator.GetEvent<AutoModeCommandOperationEvent>()
                    .Publish(new AutoModeCommandOperationEventArgs(EAutoModeCmd.On));
                _vm.AutoMode = true;
            }
        }

        private void UncheckButtonAutoMode(object sender, RoutedEventArgs e)
        {
            if (_vm.AuthCompletedFlag && (!_vm.IsNeedToAuthAgain || _vm.DevFlag))
            {
                _eventAggregator.GetEvent<AutoModeCommandOperationEvent>()
                    .Publish(new AutoModeCommandOperationEventArgs(EAutoModeCmd.Off));
                _vm.AutoMode = false;
                return;
            }

            if (AuthenticationConfirmation(Permissions.SwitchModeManualAuto))
            {
                _eventAggregator.GetEvent<AutoModeCommandOperationEvent>()
                    .Publish(new AutoModeCommandOperationEventArgs(EAutoModeCmd.Off));
                _vm.AutoMode = false;
            }
        }

        private async void SettingsButtonClick(object sender, RoutedEventArgs e)
        {
            if (_vm.AuthCompletedFlag && (!_vm.IsNeedToAuthAgain || _vm.DevFlag))
            {
                //await Task.Run(() => AppSettings.Resolve<IBrokerWrapper>().ExecuteCommand(
                //    DestinationService.CreateLocalServiceDescription(OpcServerService.ServiceId),
                //    new Command(OpcServerService.SetAutoModeCommand) { CommandData = new AutoModeCommandParams(false) })
                //);

                OpenConfigWindow();
                return;
            }
            if (AuthenticationConfirmation(Permissions.ChangeConfig))
            {
                //await Task.Run(() => AppSettings.Resolve<IBrokerWrapper>().ExecuteCommand(
                //    DestinationService.CreateLocalServiceDescription(OpcServerService.ServiceId),
                //    new Command(OpcServerService.SetAutoModeCommand) { CommandData = new AutoModeCommandParams(false) })
                //);

                OpenConfigWindow();
            }
        }

        private void OnOpenAuthWindowRequest(OpenAuthWindowRequestEventArgs args)
        {
            //закрываем окно настроек
            CloseConfigWindow();
            
            PerformFirstAuthentication();

        }

        private void OnCloseConfigWindowRequest(CloseConfigWindowRequestEventArgs args)
        {
            if (!_vm.AuthCompletedFlag)
            {
                CloseConfigWindow();
                PerformFirstAuthentication();
                return;
            }
            //закрываем окно настроек
            CloseConfigWindow();
        }

        private void PerformFirstAuthentication()
        {
            if(_vm.AuthCompletedFlag)
            {
                return;
            }
            var authWindow = new AuthenticationWindow(_authorizationService)
            {
                Owner = this,
                TitleText = "Вход в АРФА"
            };

            if (authWindow.ShowDialog() == true)
            {
                //флаг завершения аутентификации
                _vm.AuthCompletedFlag = _authorizationService.IsUserAuthorized();

                //флаг авторизации разработчика
                _vm.DevFlag = _authorizationService.IsAuthorizedAsDeveloper();
                //после успешной аутентификацию запускаем таймер на повторное подтверждение
                _timer.Start();
                return;
            }

            //если окно авторизации закрыли, то открываем окно настроек
            OpenConfigWindow();
        }

        private bool AuthenticationConfirmation(Guid action)
        {
            var authWindow = new AuthenticationWindow(_authorizationService)
            {
                Owner = this,
                OkButtonContent = "Ок",
                TitleText = "Подтверждение",
                objectId = _appConfigurationService.GetSystemId(),
                actionId = action
            };

            var login = _authorizationService.GetCurrentUserLogin();

            if (login == string.Empty)
            {
                authWindow.LoginIsEnabled = true;
            }
            else
            {
                authWindow.Login = login;
                authWindow.LoginIsEnabled = false;
            }

            return authWindow.ShowDialog() == true;
        }

        private void OpenConfigWindow()
        {
            _configPage = new ConfigView(_configViewModel);
            _configPage.Owner = this;

            //Для блокировки основного окна вызываем ShowDialog(), а не Show()
            _configPage?.ShowDialog();
        }

        private void CloseConfigWindow()
        {
            if (_configPage == null)
            {
                return;
            }

            if (_configPage.IsActive)
            {
                _configPage.Close();
            }
        }

        public void CloseSplashScreen()
        {
            if(_splashScreen != null && IsLoaded && IsVisible)
            {
                _splashScreen.Close();
                _splashScreen = null;
                Focus();
            }
        }

        public async void ShowSplashScreen(string title, int secondsToClose)
        {
            CloseSplashScreen();

            _splashScreen = new UIElements.Loading.SplashScreen(secondsToClose)
            {
                Owner = this,
                TitleText = title
                
            };
            await Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (_splashScreen != null)
                    {
                        _splashScreen.ShowDialog();
                    }
                });
            });
        }
    }
}
