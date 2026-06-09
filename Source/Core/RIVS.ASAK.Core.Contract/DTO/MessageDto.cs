using RIVS.ASAK.Core.Contract.Enums;
using System;

namespace RIVS.ASAK.Core.Contract.DTO
{
    public class MessageDto
    {
        public DateTime Timestamp { get; set; }
        public EMessageType MessageType { get; set; }
        public int Level { get; set; }
        public string Text { get; set; }
        public string Source { get; set; } = "Unknown";
    }
}
