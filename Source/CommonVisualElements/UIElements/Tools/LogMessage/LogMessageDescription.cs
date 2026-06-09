using System;
using System.Collections.Generic;
using RIVS.ASAK.Core.Contract.Enums;

namespace RIVS.ASAK.UIElements.Tools.LogMessage
{
    public static class LogMessageHelper
    {
        private static Dictionary<EMessageType, string> MessageTypeToString => new Dictionary<EMessageType, string>
                {
                    { EMessageType.None, "INFO" },
                    { EMessageType.Info, "INFO" },
                    { EMessageType.Warning, "WARN" },
                    { EMessageType.Error, "ERR" },
                    { EMessageType.Debug, "DEBUG" },
                };

        public static string GetStringByMessageType(EMessageType messageType)
        {
            return MessageTypeToString.ContainsKey(messageType) ? MessageTypeToString[messageType] : string.Empty;
        }
    }

    public interface ILogMessageDescription
    {
        DateTime Time { get; set; }
        EMessageType MessageType { get; set; }
        string Message { get; set; }
    }

    public class LogMessageDescription : ILogMessageDescription
    {
        public DateTime Time { get; set; }
        public EMessageType MessageType { get; set; }
        public string Message { get; set; }

        public LogMessageDescription(DateTime time, EMessageType messageType, string message)
        {
            Time = time;
            MessageType = messageType;
            //Message = $"{time.Date:dd-MM-yy} {time:HH:mm:ss} [{LogMessageHelper.GetStringByMessageType(messageType)}] {message}";
            //убран MessageType - он должен выдаваться в цвете(?)
            Message = message;
        }

        public LogMessageDescription(EMessageType messageType, string message)
        : this(DateTime.Now, messageType, message)
        {}

        public LogMessageDescription(string message)
            : this(DateTime.Now, EMessageType.Info, message)
        { }

    }
}
