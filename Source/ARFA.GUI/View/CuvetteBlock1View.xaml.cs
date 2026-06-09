using RIVS.ASAK.ARFA.GUI.EventArgs;
using RIVS.ASAK.ARFA.GUI.Helpers;
using RIVS.ASAK.ARFA.GUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace RIVS.ASAK.ARFA.GUI.View
{
    /// <summary>
    /// Логика взаимодействия для CuvetteBlock1View.xaml
    /// </summary>
    public partial class CuvetteBlock1View : UserControl
    {
        private const string StrCuvette = "Cuvette";
        private const string StrReper = "Reper";
        private const string StrButton = "Button";
        private readonly int _strCuvetteLength = StrCuvette.Length;
        private readonly List<System.Windows.Shapes.Path> _lstPath;
        private readonly List<Button> _lstButton;

        //private readonly object _cveEventDataReceivedLock = new object();
        //private IReceiverChannel _cveInputChannel;
        //Guid _cveSubscribeCookie;

        Color colorSelectBtn;
        Color colorUnselectBtn;

        Brush brushSelectBtn;
        Brush brushUnselectBtn;

        public CuvetteBlock1View()
        {
            InitializeComponent();
            _lstPath = new List<System.Windows.Shapes.Path>();
            _lstButton = new List<Button>();
            Loaded += CuvetteBlock1View_Loaded;
        }

        private void CuvetteBlock1View_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = (CuvetteBlock1ViewModel)DataContext;
            vm.eventAggregator.GetEvent<HighlightCuvetteChangedEvent>().Subscribe(OnHighlightCuvetteChanged);

            colorSelectBtn = (Color)ColorConverter.ConvertFromString("#FF469646");
            colorUnselectBtn = (Color)ColorConverter.ConvertFromString("#FF2D2D30");

            brushSelectBtn = new SolidColorBrush(colorSelectBtn);
            brushUnselectBtn = new SolidColorBrush(colorUnselectBtn);

            foreach (var btn in SpCuvetteBlock.Children.OfType<Button>())
            {
                _lstButton.Add(btn);
                if (btn.Content is Grid grid)
                {
                    var paths = grid.Children.OfType<System.Windows.Shapes.Path>();
                    _lstPath.AddRange(paths);
                }
            }

            //SubscribeEventFromMotor();
        }

        private void OnHighlightCuvetteChanged(HighlightCuvetteChangedEventArgs obj)
        {
            DispatcherHelper.RunAsync(() =>
            {
                if (obj.HighlightCuvette.Any())
                {
                    OnHighlightCuvetteSet(obj.HighlightCuvette);
                }
                else
                {
                    OnHighlightCuvetteClear();
                }
            }, DispatcherPriority.Background);
        }

        private void OnHighlightCuvetteClear()
        {
            foreach (var path in _lstPath.Where(path => !path.Name.Contains(StrReper)))
            {
                path.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }
        }

        private void OnHighlightCuvetteSet(List<int> lst)
        {
            foreach (var path in from path in _lstPath
                                 where !path.Name.Contains(StrReper) && path.Name.Contains(StrCuvette)
                                 let idx = path.Name.IndexOf(StrCuvette, StringComparison.CurrentCulture)
                                 let strCuvetteNumber = path.Name.Remove(0, idx + _strCuvetteLength)
                                 let cuvetteNumber = int.Parse(strCuvetteNumber)
                                 where lst.Contains(cuvetteNumber)
                                 select path)
            {
                path.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 255, 255));
            }
        }

        //private void SubscribeEventFromMotor()
        //{
        //    var conditionEvent = new EventFilter();
        //    conditionEvent.Types.Add(EventTypes.MotorMoveToTablePositionFinish);
        //    conditionEvent.Types.Add(EventTypes.MotorGoHomeStart);

        //    AppSettings.Resolve<IBlackboardWrapper>().Subscribe(
        //        conditionEvent,
        //        out _cveInputChannel,
        //        OnCveEventDataReceived,
        //        out _cveSubscribeCookie);

        //    _cveInputChannel.DataReceived += OnCveEventDataReceived;
        //}

        //private void OnCveEventDataReceived(IReceiverChannel channel)
        //{
        //    lock (_cveEventDataReceivedLock)
        //    {
        //        channel.GetData(out var chData);
        //        if (chData is not Event eventData)
        //        {
        //            return;
        //        }

        //        if (eventData.Type == EventTypes.MotorMoveToTablePositionFinish)
        //        {
        //            var commandDone = (CommandDataEvent.CommandDoneEventData)eventData.ExtData;
        //            if (commandDone.ResultCode != CommandDataEvent.CommandResultCode.OK)
        //            {
        //                return;
        //            }

        //            var motorMoveToTablePositionEventData = (MotorMoveToTablePositionEventData)commandDone.CommandResult;
        //            var cuvette = (int)motorMoveToTablePositionEventData.Cuvette;

        //            Dispatcher.BeginInvoke(new Action(() =>
        //            {
        //                var selectBtn = _lstButton.Where(x => (SolidColorBrush)x.Background == brushSelectBtn);
        //                foreach (var item in selectBtn)
        //                {
        //                    item.Background = brushUnselectBtn;
        //                }

        //                foreach (var item in from item in _lstButton 
        //                         let idx = item.Name.IndexOf(StrButton, StringComparison.CurrentCulture) 
        //                         let strButtonNumber = item.Name.Remove(0, idx + StrButton.Length) 
        //                         let buttonNumber = int.Parse(strButtonNumber) 
        //                         where buttonNumber == cuvette 
        //                         select item)
        //                {
        //                    item.Background = brushSelectBtn;
        //                    break;
        //                }
        //            }));

        //        }
        //        else if (eventData.Type == EventTypes.MotorGoHomeStart)
        //        {
        //            Dispatcher.BeginInvoke(new Action(() =>
        //            {
        //                var selectBtn = _lstButton.Where(x => (SolidColorBrush)x.Background == brushSelectBtn);
        //                foreach (var item in selectBtn)
        //                {
        //                    item.Background = brushUnselectBtn;
        //                }
        //            }));
        //        }
        //    }
        //}

    }
}
