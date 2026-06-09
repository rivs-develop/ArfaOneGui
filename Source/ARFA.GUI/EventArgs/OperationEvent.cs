using Prism.Events;

namespace RIVS.ASAK.ARFA.GUI.EventArgs
{

    public class OperationEvent : PubSubEvent<OperationEventArgs>
    {
    }

    public class OperationEventArgs
    {

        public OperationEventArgs()
        {
        }
    }
}
