using System;

namespace RIVS.ASAK.Core.Contract.DTO
{

    public class ContourDTO
    {
        public int Number { get; set; }
        public Guid ContourGuid { get; set; }
        public short Cluster { get; set; }
        public bool IsUse { get; set; }

        public ContourDTO()
        {
        }
    }
}
