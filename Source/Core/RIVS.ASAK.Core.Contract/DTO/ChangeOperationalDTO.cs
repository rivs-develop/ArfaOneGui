using System;

namespace RIVS.ASAK.Core.Contract.DTO
{
    public class ChangeOperationalDTO
    {
        public DateTime Date { get; set; }
        public string Parameter { get; set; } = string.Empty;
        public int LineNum { get; set; }
        public Guid DeviceGuid { get; set; }
        public string NewValue { get; set; } = string.Empty;

        public ChangeOperationalDTO() { }
    }
}
