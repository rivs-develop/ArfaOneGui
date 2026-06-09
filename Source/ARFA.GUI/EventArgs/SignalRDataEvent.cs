using Prism.Events;
using RIVS.ASAK.Core.Contract.DTO;
using System.Collections.Generic;

namespace RIVS.ASAK.ARFA.GUI.EventArgs
{
    // Существующие события
    public class SystemStateChangedEvent : PubSubEvent<SystemStateData> { }
    public class ConnectionStatusChangedEvent : PubSubEvent<bool> { }

    // События для сообщений
    public class MessageReceivedEvent : PubSubEvent<MessageDto> { }
    public class MessagesReceivedEvent : PubSubEvent<IEnumerable<MessageDto>> { }
    public class ImportantMessagesReceivedEvent : PubSubEvent<IEnumerable<MessageDto>> { }

    public class ApplicationStateReceivedEvent : PubSubEvent<ApplicationStateDto> { }
}
