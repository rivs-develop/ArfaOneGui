using System;

namespace RIVS.ASAK.Core.Contract.DTO
{
    public class DeviceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }
        public int TypeId { get; set; }
        public Guid DeviceId { get; set; }
        public Guid CyclogramId { get; set; }
        public bool IsUse { get; set; }

        public DeviceDTO() { }

        public override string ToString()
        {
            return $"{{{nameof(Name)}={Name}, {nameof(TypeId)}={TypeId}, {nameof(Ip)}={Ip}, {nameof(Port)}={Port}}}";
        }
    }
}
