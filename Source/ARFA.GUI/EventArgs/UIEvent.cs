using Prism.Events;

namespace RIVS.ASAK.ARFA.GUI.EventArgs
{
    public class OpenAuthWindowRequestEvent : PubSubEvent<OpenAuthWindowRequestEventArgs>
    {
    }

    public class OpenAuthWindowRequestEventArgs
    {
        public OpenAuthWindowRequestEventArgs()
        {
        }
    }

    public class CloseConfigWindowRequestEvent : PubSubEvent<CloseConfigWindowRequestEventArgs>
    {
    }

    public class CloseConfigWindowRequestEventArgs
    {
        public CloseConfigWindowRequestEventArgs()
        {
        }
    }
    

    public class AuthCompletedFlagChangedEvent : PubSubEvent<AuthCompletedFlagChangedEventArgs>
    {
    }

    public class AuthCompletedFlagChangedEventArgs
    {
        public bool NewFlag { get; private set; }
        public AuthCompletedFlagChangedEventArgs(bool newFlag)
        {
            NewFlag = newFlag;
        }
    }

    public class DevFlagChangedEvent : PubSubEvent<DevFlagChangedEventArgs>
    {
    }

    public class DevFlagChangedEventArgs
    {
        public bool NewFlag { get; private set; }
        public DevFlagChangedEventArgs(bool newFlag)
        {
            NewFlag = newFlag;
        }
    }
}
