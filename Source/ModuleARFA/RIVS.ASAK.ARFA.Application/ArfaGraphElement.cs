namespace RIVS.ASAK.ARFA.Application
{
    /// <summary>
    /// Данные из Elements, которые есть в АП
    /// </summary>
    public class ArfaGraphElement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Start { get; set; }
        public int Stop { get; set; }
        public string Line { get; set; }

        public ArfaGraphElement(int id, string name, int start, int stop, string line)
        {
            Id = id;
            Name = name;
            Start = start;
            Stop = stop;
            Line = line;
        }
    }
}
