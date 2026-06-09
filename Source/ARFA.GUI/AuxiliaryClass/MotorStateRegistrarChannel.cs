namespace RIVS.ASAK.ARFA.GUI.AuxiliaryClass
{
    //public sealed class MotorStateRegistrarChannel : ISenderChannel, IReceiverChannel, ICommunicationChannel
    //{
    //    private readonly Queue<object> _deque = new Queue<object>();
    //    private bool _isRunning;
    //    internal MotorStateRegistrarChannel()
    //    {
    //        Open();
    //    }

    //    //public void SetData(object data)
    //    //{
    //    //    var dataResult = (TubeX2StatusData)data;
    //    //    XrayVoltage = (double)dataResult.Voltage;

    //    //    //    DppRealTime = Math.Round(dppInternalState.RealTime);
    //    //    //    DppTemperature = dppInternalState.PlateTemp;
    //    //    //    DppCurrentDT = Math.Round(dppInternalState.DeadTime, 1);
    //    //}

    //    #region ISenderChannel Members

    //    public void SetData(object data)
    //    {
    //        if (_isRunning)
    //        {
    //            if (data is MotorStatusData)
    //            {
    //                lock (_deque)
    //                {
    //                    _deque.Enqueue(data);
    //                }

    //                var receiver = DataReceived;
    //                if (receiver != null)
    //                {
    //                    receiver.BeginInvoke(this, null, null); // _асинхронно_ уведомляем о новых данных
    //                }

    //                if (_deque.Count >= 40 && _deque.Count % 5 == 0)
    //                {
    //                    //_logger.Warn("!!! Closer - Большая очередь результатов - {0}", _deque.Count);
    //                }
    //            }
    //            else
    //            {
    //                //Debug.Assert(false, "Неожидаемый тип входных данных");
    //            }
    //        }
    //    }

    //    #endregion

    //    #region IReceiverChannel Members

    //    public void GetData(out object data)
    //    {
    //        lock (_deque)
    //        {
    //            data = _deque.Dequeue();
    //        }
    //    }

    //    public event DataReceiveEventHandler DataReceived;

    //    #endregion

    //    #region ICommunicationChannel Members

    //    public void Open()
    //    {
    //        _isRunning = true;
    //    }

    //    public void Close()
    //    {
    //        _isRunning = false;

    //        lock (_deque)
    //        {
    //            _deque.TakeWhile(item => { item = null; return true; }).Count();
    //            _deque.Clear();
    //        }
    //    }

    //    public bool IsOpen
    //    {
    //        get { return _isRunning; }
    //    }


    //    public Guid ID
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public ConnectionState State
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    event ConnectionStateChangedHandler ICommunicationChannel.StateChanged
    //    {
    //        add { throw new NotImplementedException(); }
    //        remove { throw new NotImplementedException(); }
    //    }

    //    public bool IsConnected()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public event ConnectionErrorEventHandler ConnectionError;

    //    public event ConnectionClosedEventHandler Closed;

    //    #endregion
    //}
}
