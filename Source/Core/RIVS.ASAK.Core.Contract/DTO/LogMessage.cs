using RIVS.ASAK.Core.Contract.Enums;
using System;

namespace RIVS.ASAK.Core.Contract.DTO
{
    public class LogMessage
    {
        public DateTime Time { get; set; }
        public EMessageType MessageType { get; set; }
        public string Message { get; set; }

        public LogMessage(DateTime time, EMessageType messageType, string message)
        {
            Time = time;
            MessageType = messageType;
            Message = message;
        }

        public LogMessage(EMessageType messageType, string message)
            : this(DateTime.Now, messageType, message)
        { }

        public LogMessage(string message)
            : this(DateTime.Now, EMessageType.Info, message)
        { }

    }
}
