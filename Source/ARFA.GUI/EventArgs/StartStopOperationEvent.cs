using Prism.Events;

namespace RIVS.ASAK.ARFA.GUI.EventArgs
{

    public class StartCommandOperationEvent : PubSubEvent<StartCommandOperationEventArgs>
    {
    }

    public class StartCommandOperationEventArgs
    {
        public StartCommandOperationEventArgs()
        {
        }
    }

    /// <summary>
    /// Возвращаемый результат выполнения команды StartCommand
    /// </summary>
    public class StartCommandOperationResultEvent : PubSubEvent<StartCommandOperationResultEventArgs>
    {
    }

    public class StartCommandOperationResultEventArgs
    {
        public bool CmdResult { get; private set; }
        public StartCommandOperationResultEventArgs(bool cmdResult)
        {
            CmdResult = cmdResult;
        }
    }


    public class StopCommandOperationEvent : PubSubEvent<StopCommandOperationEventArgs>
    {
    }

    public class StopCommandOperationEventArgs
    {
        public StopCommandOperationEventArgs()
        {
        }
    }

    /// <summary>
    /// Возвращаемый результат выполнения команды StopCommand
    /// </summary>
    public class StopCommandOperationResultEvent : PubSubEvent<StopCommandOperationResultEventArgs>
    {
    }

    public class StopCommandOperationResultEventArgs
    {
        public bool CmdResult { get; private set; }
        public StopCommandOperationResultEventArgs(bool cmdResult)
        {
            CmdResult = cmdResult;
        }
    }
}
