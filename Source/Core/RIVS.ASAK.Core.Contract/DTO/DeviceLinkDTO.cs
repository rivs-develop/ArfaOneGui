using System;

namespace RIVS.ASAK.Core.Contract.DTO
{
    public class DeviceLinkDTO
    {
        public string DeviceName { get; set; }
        public Guid DeviceGuid { get; set; }
        public string ParentDeviceName { get; set; }
        public Guid ParentDeviceGuid { get; set; }

        public DeviceLinkDTO() { }

        public override string ToString()
        {
            return $"{{DeviceName={DeviceName}, DeviceGuid={DeviceGuid}, ParentDeviceName={ParentDeviceName}, ParentDeviceGuid={ParentDeviceGuid}}}";
        }
    }
}
