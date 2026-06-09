using System;

namespace RIVS.ASAK.Core.Contract.DTO
{
    public class AnalyticProgramDTO
    {
        public int Id { get; set; }
        public DateTime EditedDT { get; set; }
        public DateTime ActivedDT { get; set; }
        public string FullName { get; set; }
        public string FileName { get; set; }
        public byte ActiveProj { get; set; }
        public string XML_TXT { get; set; }
        public string AnalyticProgramHash { get; set; }
    }
}
