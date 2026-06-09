namespace RIVS.ASAK.Core.Contract.DTO
{
    public class CyclogramParameterDTO
    {
        public int CyclogramId { get; set; }
        public string ParameterName { get; set; }
        public int Value { get; set; }

        public CyclogramParameterDTO() { }

        public override string ToString()
        {
            return $"CyclogramId: {CyclogramId}, ParameterName: {ParameterName}, Value: {Value}";
        }
    }
}
