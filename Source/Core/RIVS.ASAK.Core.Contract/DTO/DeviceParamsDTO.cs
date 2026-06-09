using System;

namespace RIVS.ASAK.Core.Contract.DTO
{

    public class DeviceParamsDTO
    {
        public string DeviceName { get; set; }
        public string DeviceIp { get; set; }
        public string DevicePort { get; set; }
        public int TypeId { get; set; }
        public Guid DeviceGuid { get; set; }
        public string ParameterDescription { get; set; }
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public int Line { get; set; }
        public Guid ContourGuid { get; set; }
        public bool IsUse { get; set; }

        public DeviceParamsDTO() { }

        public override string ToString()
        {
            return $"{{DeviceName={DeviceName}, DeviceIp={DeviceIp}, DevicePort={DevicePort}, TypeId={TypeId}, DeviceGuid={DeviceGuid}, ParameterDescription={ParameterDescription}, ParameterName={ParameterName}, ParameterValue={ParameterValue}, Line={Line}, ContourGuid={ContourGuid}, IsUse={IsUse}}}";
        }
    }
}
