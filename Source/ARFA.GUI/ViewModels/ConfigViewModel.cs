using System.Collections.ObjectModel;
using System.Windows.Controls;
using Prism.Events;
using RIVS.ASAK.ARFA.GUI.EventArgs;
using RIVS.ASAK.ARFA.GUI.Model;
using RIVS.ASAK.ARFA.GUI.View;
using RIVS.ASAK.Core.Tools.WinApi;

namespace RIVS.ASAK.ARFA.GUI.ViewModels
{
    
    public class ConfigViewModel : BaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        const int MaxApNameLength = 16;

        private bool _devFlag;
        public bool DevFlag
        {
            get => _devFlag;
            set
            {
                _devFlag = value;
                OnPropertyChanged(nameof(DevFlag));
                SetAuthButtonSepearatorHeight();
                SetPowerButtonSepearatorHeight();
            }
        }

        private bool _authCompletedFlag;
        public bool AuthCompletedFlag
        {
            get => _authCompletedFlag;
            set
            {
                _authCompletedFlag = value;
                OnPropertyChanged(nameof(AuthCompletedFlag));
                SetAuthButtonSepearatorHeight();
            }
        }

        private int _authButtonSepearatorHeight;
        public int AuthButtonSepearatorHeight
        {
            get => _authButtonSepearatorHeight;
            set
            {
                _authButtonSepearatorHeight = value;
                OnPropertyChanged(nameof(AuthButtonSepearatorHeight));
            }
        }

        private int _powerButtonSepearatorHeight;
        public int PowerButtonSepearatorHeight
        {
            get => _powerButtonSepearatorHeight;
            set
            {
                _powerButtonSepearatorHeight = value;
                OnPropertyChanged(nameof(PowerButtonSepearatorHeight));
            }
        }

        private bool _returnToMainWindowButtonIsEnabled;

        public bool ReturnToMainWindowButtonIsEnabled
        {
            get => _returnToMainWindowButtonIsEnabled;
            set
            {
                _returnToMainWindowButtonIsEnabled = value;
                OnPropertyChanged(nameof(ReturnToMainWindowButtonIsEnabled));
            }
        }

        private bool _powerPageButtonIsEnabled = true;
        public bool PowerPageButtonIsEnabled
        {
            get => _powerPageButtonIsEnabled;
            set
            {
                _powerPageButtonIsEnabled = value;
                OnPropertyChanged(nameof(PowerPageButtonIsEnabled));
            }
        }

        private bool _configPageButtonIsEnabled = true;
        public bool ConfigPageButtonIsEnabled
        {
            get => _configPageButtonIsEnabled;
            set
            {
                _configPageButtonIsEnabled = value;
                OnPropertyChanged(nameof(ConfigPageButtonIsEnabled));
            }
        }

        private bool _authWindowButtonIsEnabled = true;
        public bool AuthWindowButtonIsEnabled
        {
            get => _authWindowButtonIsEnabled;
            set
            {
                _authWindowButtonIsEnabled = value;
                OnPropertyChanged(nameof(AuthWindowButtonIsEnabled));
            }
        }

        private Page _slideFrame;

        public Page SlideFrame
        {
            get => _slideFrame;
            set
            {
                _slideFrame = value;
                OnPropertyChanged(nameof(SlideFrame));
            }
        }

        private string _apName;
        public string ApName
        {
            get { return _apName; }
            set
            {
                if (value == null)
                {
                    _apName = null;
                    return;
                }
                //отрезаем .xml
                var val = value.Replace(".xml", string.Empty);

                if (val.Length > MaxApNameLength)
                {
                    //вычитаем от максимального значения 3 символа для 3-ёх точек
                    var maxLength = MaxApNameLength - 3;
                    //проверяем на отрицательное число
                    if (maxLength < 0)
                    {
                        maxLength = 0;
                    }
                    _apName = $"{value.Remove(maxLength, value.Length - maxLength)}...";
                }
                else
                {
                    _apName = val;
                }
                OnPropertyChanged(nameof(ApName));
            }
        }

        private ObservableCollection<ElementModel> _activeElementsCollection = new ObservableCollection<ElementModel>();
        public ObservableCollection<ElementModel> ActiveElementsCollection
        {
            get => _activeElementsCollection;
            set
            {
                _activeElementsCollection = value;
                OnPropertyChanged(nameof(ActiveElementsCollection));
            }
        }

        //public event EventHandler OpenAuthWindowRequest;

        //public event EventHandler CloseConfigWindowRequest;

        private Reboot _reboot;

        public ConfigViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            SetAuthButtonSepearatorHeight();
            SetPowerButtonSepearatorHeight();
            ReturnToMainWindowButtonIsEnabled = true;

            _eventAggregator.GetEvent<AnalyticalProgramChangedEvent>().Subscribe(OnAnalyticalProgramChangedEvent);
        }


        public void OnAuthCompletedFlagChanged(AuthCompletedFlagChangedEventArgs args)
        {
            AuthCompletedFlag = args.NewFlag;
        }

        public void OnDevFlagChanged(DevFlagChangedEventArgs args)
        {
            DevFlag = args.NewFlag;
        }

        private void OnAnalyticalProgramChangedEvent(AnalyticalProgramChangedEventArgs eventArgs)
        {
            if (eventArgs.analyticalProjectsOn)
            {
                ApName = eventArgs.Name;
            }
            else
            {
                ApName = string.Empty;
            }
        }

        private void SetAuthButtonSepearatorHeight()
        {
            if(DevFlag)
            {
                AuthButtonSepearatorHeight = 82;
                return;
            }
            AuthButtonSepearatorHeight = AuthCompletedFlag ? 154 : 82;
        }

        private void SetPowerButtonSepearatorHeight()
        {
            PowerButtonSepearatorHeight = DevFlag ? 254 : 334;
        }

        internal void OpenAuthWindow()
        {
            //OpenAuthWindowRequest?.Invoke(this, System.EventArgs.Empty);
            _eventAggregator.GetEvent<OpenAuthWindowRequestEvent>().Publish(new OpenAuthWindowRequestEventArgs());
        }

        internal void CloseConfigWindow()
        {
            //CloseConfigWindowRequest?.Invoke(this, System.EventArgs.Empty);
            _eventAggregator.GetEvent<CloseConfigWindowRequestEvent>().Publish(new CloseConfigWindowRequestEventArgs());
        }

        private async void SendCommandStop()
        {
            ReturnToMainWindowButtonIsEnabled = false;

            //отключение устройств
            bool resultStop;

            //TODO: тут отправляем стоп

            //var resultStopDevices = ;

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
        }

        //TODO: нужно отправлять команду Stop
        //и ожидать событие ответ  StartCommandOperationResultEvent
        internal void AppExit()
        {
            SlideFrame = new AppExitConfigPage();
            ToggleFrameButtonsClickability(false);
            SendCommandStop();

            var app = System.Windows.Application.Current;
            app.Shutdown();
        }

        //TODO: нужно отправлять команду Stop
        //и ожидать событие ответ  StopCommandOperationResultEvent
        internal void ArfaRestart()
        {
            SlideFrame = new AppExitConfigPage();
            ToggleFrameButtonsClickability(false);
            SendCommandStop();

            _reboot ??= new Reboot();

            _reboot.halt(true, true);
        }

        //TODO: нужно отправлять команду Stop
        //и ожидать событие ответ  StopCommandOperationResultEvent
        internal void ArfaShutdown()
        {
            SlideFrame = new AppExitConfigPage();
            ToggleFrameButtonsClickability(false);
            SendCommandStop();

            _reboot ??= new Reboot();

            _reboot.halt(false, true);

        }

        internal void UpdateAp()
        {
            //логика обновления АП
        }

        internal void UpdateElements()
        {
            //логика обновления элементов
        }

        public void ToggleFrameButtonsClickability(bool isEnabled)
        {
            ReturnToMainWindowButtonIsEnabled = isEnabled;
            PowerPageButtonIsEnabled = isEnabled;
            AuthWindowButtonIsEnabled = isEnabled;
            ConfigPageButtonIsEnabled = isEnabled;
        }
    }
}
