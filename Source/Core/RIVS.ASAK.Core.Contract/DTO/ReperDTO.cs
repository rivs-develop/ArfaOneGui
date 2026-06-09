using System;
using System.Collections.Generic;

namespace RIVS.ASAK.Core.Contract.DTO
{
    public class ElementJaDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Ja { get; set; }
    }

    public class ReperDTO
    {
        public int Id { get; set; }
        public DateTime Meas_DT { get; set; }
        public int Pribor { get; set; }
        public int Cuvet { get; set; }
        public sbyte Kz { get; set; }
        public int ReperHashKey { get; set; }
        public IEnumerable<ElementJaDTO> ElementJas { get; set; }
    }
}
