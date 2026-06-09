using Prism.Events;
using RIVS.ASAK.Core.Contract.DTO;
using System.Collections.Generic;

namespace RIVS.ASAK.ARFA.GUI.EventArgs
{
    // События для сообщений
    public class ImportantMessagesUpdatedEvent : PubSubEvent<IEnumerable<MessageDto>> { }
    public class GeneralMessagesUpdatedEvent : PubSubEvent<IEnumerable<MessageDto>> { }
}
