using System;

namespace RIVS.ASAK.Core.Contract.DTO
{
    public class ProductDTO
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public int Kuvet { get; set; }
        public int CyclogramNumber { get; set; }
        public Guid CyclogramId { get; set; }
        public Guid ProductId { get; set; }
        public string Arfa { get; set; }
        public Guid ArfaId { get; set; }
        public string Akcp { get; set; }
        public Guid AkcpId { get; set; }
        public string Akp { get; set; }
        public Guid AkpId { get; set; }
        public string Apk { get; set; }
        public Guid ApkId { get; set; }
        public bool IsUse { get; set; }

        public ProductDTO() { }

        public override string ToString()
        {
            return $"{{{nameof(Name)}={Name}, {nameof(Number)}={Number}, {nameof(CyclogramId)}={CyclogramId}}}";
        }
    }
}
