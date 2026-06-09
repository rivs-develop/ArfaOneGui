using System;
using Prism.Events;
using RIVS.ASAK.ARFA.GUI.Enums;
using RIVS.ASAK.ARFA.GUI.EventArgs;
using RIVS.ASAK.ARFA.GUI.Services;

namespace RIVS.ASAK.ARFA.GUI.ViewModels
{

    public class ParametersViewModel
        : BaseViewModel
            , IParametersViewModel
    {
        //константа должна быть >= 4
        const int MaxApNameLength = 13;
        private readonly IEventAggregator _eventAggregator;

        //readonly XrayStateRegistrarChannel _xrayStateRegistrarChannel;

        //readonly DppStateRegistrarChannel _dppStateRegistrarChannel;

        //private readonly MotorStateRegistrarChannel _motorStateRegistrarChannel;

        private EState _analyticalProgramState;
        public EState AnalyticalProgramState
        {
            get { return _analyticalProgramState; }
            set
            {
                _analyticalProgramState = value;
                OnPropertyChanged(nameof(AnalyticalProgramState));
            }
        }

        private string _arfaNum;
        public string ArfaNum
        {
            get { return _arfaNum; }
            set
            {
                _arfaNum = value;
                OnPropertyChanged(nameof(ArfaNum));
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
                var val = value.Replace(".xml", String.Empty);

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

        private string _apModifyDate;
        public string ApModifyDate
        {
            get { return _apModifyDate; }
            set
            {
                _apModifyDate = value;
                OnPropertyChanged(nameof(ApModifyDate));
            }
        }

        private EState _xrayState;
        public EState XrayState
        {
            get { return _xrayState; }
            set
            {
                SetValue(ref _xrayState, value);
            }
        }

        private double _xrayVoltage;
        public double XrayVoltage
        {
            get { return _xrayVoltage; }
            set
            {
                SetValue(ref _xrayVoltage, value);
            }
        }

        private double _xrayCurrent;
        public double XrayCurrent
        {
            get { return _xrayCurrent; }
            set
            {
                SetValue(ref _xrayCurrent, value);
            }
        }

        private double _xrayTemperature;
        public double XrayTemperature
        {
            get { return _xrayTemperature; }
            set
            {
                SetValue(ref _xrayTemperature, value);
            }
        }

        private EState _dppState;
        public EState DppState
        {
            get { return _dppState; }
            set
            {
                SetValue(ref _dppState, value);
            }
        }

        private double _dppRealTime;
        public double DppRealTime
        {
            get { return _dppRealTime; }
            set
            {
                SetValue(ref _dppRealTime, value);
            }
        }

        private double _dppCurrentDT;
        public double DppCurrentDT
        {
            get { return _dppCurrentDT; }
            set
            {
                SetValue(ref _dppCurrentDT, value);
            }
        }

        private double _dppTemperature;
        public double DppTemperature
        {
            get { return _dppTemperature; }
            set
            {
                SetValue(ref _dppTemperature, value);
            }
        }

        private EState _cveState;
        public EState CveState
        {
            get { return _cveState; }
            set
            {
                SetValue(ref _cveState, value);
            }
        }

        
        private long _cveCurrentRecord;
        public long CveCurrentRecord
        {
            get { return _cveCurrentRecord; }
            set
            {
                SetValue(ref _cveCurrentRecord, value);
            }
        }

        private double _cveActualPosition;
        public double CveActualPosition
        {
            get { return _cveActualPosition; }
            set
            {
                SetValue(ref _cveActualPosition, value);
            }
        }

        private double _cveTargetPosition;
        public double CveTargetPosition
        {
            get { return _cveTargetPosition; }
            set
            {
                SetValue(ref _cveTargetPosition, value);
            }
        }


        public ParametersViewModel(IEventAggregator eventAggregator,
            IArfaConfigurationResolver arfaConfigurationResolver)
        {

            _analyticalProgramState = EState.None;
            _apName = string.Empty;
            _apModifyDate= string.Empty;

            _xrayState = EState.None;
            _xrayVoltage = 0.0;
            _xrayCurrent = 0.0;
            _xrayTemperature = 0.0;

            _cveState = EState.None;
            _cveCurrentRecord = 0L;

            _dppState = EState.None;

            //_cveActualPosition = 0.0;

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<AnalyticalProgramChangedEvent>().Subscribe(OnAnalyticalProgramChangedEvent);

            ArfaNum = arfaConfigurationResolver.GetArfaNumber().ToString();

            //_xrayStateRegistrarChannel = new XrayStateRegistrarChannel();
            //_xrayStateRegistrarChannel.DataReceived += OnXrayDataReceived;

            //_dppStateRegistrarChannel = new DppStateRegistrarChannel();
            //_dppStateRegistrarChannel.DataReceived += OnDppDataReceived;

            //_motorStateRegistrarChannel = new MotorStateRegistrarChannel();
            //_motorStateRegistrarChannel.DataReceived += OnMotorDataReceived;
        }

        //private void OnXrayDataReceived(IReceiverChannel channel)
        //{
        //    Debug.Assert(channel != null);

        //    channel.GetData(out var data);
        //    var dataResult = (TubeX2StatusData)data;
        //    if (dataResult == null)
        //    {
        //        return;
        //    }
        //    XrayVoltage = Math.Round(dataResult.Voltage, 2, MidpointRounding.AwayFromZero);
        //    XrayCurrent = Math.Round(dataResult.Current, 2, MidpointRounding.AwayFromZero);
        //    XrayTemperature = Math.Round(dataResult.Temp, 2, MidpointRounding.AwayFromZero);
        //}

        //private void OnDppDataReceived(IReceiverChannel channel)
        //{
        //    Debug.Assert(channel != null);

        //    channel.GetData(out var data);
        //    var dataResult = (DetectorStatusData)data;
        //    if (dataResult == null)
        //    {
        //        return;
        //    }
        //    DppRealTime = dataResult.RealTime;
        //    DppTemperature = dataResult.Temperature;
        //    DppCurrentDT = dataResult.DeadTime;

        //}

        //private void OnMotorDataReceived(IReceiverChannel channel)
        //{
        //    Debug.Assert(channel != null);

        //    channel.GetData(out var data);
        //    var dataResult = (MotorStatusData)data;
        //    if (dataResult == null)
        //    {
        //        return;
        //    }

        //    //заглушка, т.к. контроллер в положении home выдает 253
        //    if(dataResult.CurrentRecord >= 253)
        //    {
        //        CveCurrentRecord = 0;
        //        CveActualPosition = 0.00;
        //        CveTargetPosition = 0.00;
        //    }
        //    else
        //    {
        //        if (dataResult.CurrentRecord > 0)
        //        {
        //            CveCurrentRecord = dataResult.CurrentRecord;
        //        }

        //        if (dataResult.ActualPosition > 0)
        //        {
        //            CveActualPosition = dataResult.ActualPosition;
        //        }

        //        if (dataResult.TargetPosition > 0)
        //        {
        //            CveTargetPosition = dataResult.TargetPosition;
        //        }
        //    }

        //}

        private void OnAnalyticalProgramChangedEvent(AnalyticalProgramChangedEventArgs eventArgs)
        {
            if (eventArgs.analyticalProjectsOn)
            {
                AnalyticalProgramState = EState.Ok;
                ApName = eventArgs.Name;
                ApModifyDate = $"от {eventArgs.dtActived:G}";
            }
            else
            {
                AnalyticalProgramState = EState.None;
                ApName = string.Empty;
                ApModifyDate = string.Empty;
            }
        }


        //public ISenderChannel GetTubeStateChannels()
        //{
        //    return _xrayStateRegistrarChannel;
        //}

        //public ISenderChannel GetDetectorStateChannels()
        //{
        //    return _dppStateRegistrarChannel;
        //}

        //public ISenderChannel GetMotorStateChannels()
        //{
        //    return _motorStateRegistrarChannel;
        //}
    }
}
