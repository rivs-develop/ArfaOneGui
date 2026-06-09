namespace RIVS.ASAK.ARFA.GUI.AuxiliaryClass
{
    public interface IConcentrationDescription
    {
        string Time { get; set; }
        string Kuvet { get; set; }
        string Prod { get; set; }
        string Conc { get; set; }
    }

    public class ConcentrationDescription : IConcentrationDescription
    {
        public string Time { get; set; }
        public string Kuvet { get; set; }
        public string Prod { get; set; }
        public string Conc { get; set; }

        public ConcentrationDescription(string time, string kuvet, string prod, string conc)
        {
            Time = time;
            Kuvet = kuvet;
            Prod = prod;
            Conc = conc;
        }
    }

}
