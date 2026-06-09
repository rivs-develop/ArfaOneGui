namespace RIVS.ASAK.Core.Contract.DTO
{
    public class ElementDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Start { get; set; }
        public int Stop { get; set; }
        public int R01 { get; set; }
        public int R02 { get; set; }
        public string Line { get; set; }
        public int Flag { get; set; }
        public float CeN { get; set; }

        public ElementDTO() { }
    }
}
