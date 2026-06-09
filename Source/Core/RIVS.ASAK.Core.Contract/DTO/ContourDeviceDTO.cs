using System;

namespace RIVS.ASAK.Core.Contract.DTO
{
    public class ContourDeviceDTO
    {
        public int Contour { get; set; }
        public int Line { get; set; }
        public int Kuvet { get; set; }
        public string ProductName { get; set; }
        public string ProductSamplingPoint { get; set; }
        public bool IsUse { get; set; }
        public string DeviceName { get; set; }
        public int DeviceTypeId { get; set; }
        public Guid DeviceGuid { get; set; }
        public Guid ContourGuid { get; set; }

        public ContourDeviceDTO()
        {
        }
    }
}
