using System;
using System.Collections.Generic;

namespace RIVS.ASAK.Core.Contract.DTO
{
    public class CyclogramDTO
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public int Arfa { get; set; }
        public int DiscretTime { get; set; }
        public int ReperStart { get; set; }
        public int ReperExpos { get; set; }
        public int GoHomeTime { get; set; }
        public int SetPositionTime { get; set; }
        public int NumberProducts { get; set; }
        public Guid CyclogramGuid { get; set; }
        public bool IsUse { get; set; }

        public IEnumerable<CyclogramDeviceTimingDTO> CyclogramDeviceTimingDTO { get; set; }
        public IEnumerable<CyclogramParameterDTO> CyclogramParameterDTO { get; set; }

        public CyclogramDTO() 
        {
        }

        public override string ToString()
        {
            return $"{{{nameof(Id)}={Id}, {nameof(CreateTime)}={CreateTime}, {nameof(Arfa)}={Arfa}, {nameof(DiscretTime)}={DiscretTime}, {nameof(ReperStart)}={ReperStart}, {nameof(ReperExpos)}={ReperExpos}, {nameof(GoHomeTime)}={GoHomeTime}, {nameof(SetPositionTime)}={SetPositionTime}, {nameof(NumberProducts)}={NumberProducts}}}";
        }
    }
 }
