using Prism.Events;

namespace RIVS.ASAK.ARFA.GUI.EventArgs
{
    public class InitializeGuiRequest : PubSubEvent<InitializeGuiRequestArgs>
    {
    }

    public class InitializeGuiRequestArgs
    {
        public InitializeGuiRequestArgs()
        {
        }

    }

    public class InitializeGuiRequestResult : PubSubEvent<InitializeGuiRequestResultArgs>
    {
    }

    public class InitializeGuiRequestResultArgs
    {
        public bool Result { get; set; }
        public bool State { get; set; }
        public InitializeGuiRequestResultArgs(bool result, bool state)
        {
            Result = result;
            State = state;
        }

    }
}
