using Prism.Events;

namespace RIVS.ASAK.ARFA.GUI.EventArgs
{
    public enum EAutoModeCmd
    {
        On = 1,
        Off,
        OutsideCmd
    }

    public class AutoModeCommandOperationEvent : PubSubEvent<AutoModeCommandOperationEventArgs>
    {
    }

    public class AutoModeCommandOperationEventArgs
    {
        public EAutoModeCmd Cmd { get; private set; }
        public AutoModeCommandOperationEventArgs(EAutoModeCmd cmd)
        {
            Cmd = cmd;
        }
    }

   
    /// <summary>
    /// Результат выполнения команды AutoModeCommand
    /// </summary>
    public class AutoModeCommandOperationResultEvent : PubSubEvent<AutoModeCommandOperationResultEventArgs>
    {
    }

    public class AutoModeCommandOperationResultEventArgs
    {
        public EAutoModeCmd Cmd { get; private set; }
        public bool CmdResult { get; private set; }
        public bool IsCmdErrorResult { get; private set; }
        public AutoModeCommandOperationResultEventArgs(EAutoModeCmd cmd, bool cmdResult, bool isCmdErrorResult)
        {
            Cmd = cmd;
            CmdResult = cmdResult;
            IsCmdErrorResult = isCmdErrorResult;
        }
    }
}
