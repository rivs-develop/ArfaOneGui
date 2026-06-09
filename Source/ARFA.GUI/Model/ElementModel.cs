namespace RIVS.ASAK.ARFA.GUI.Model
{
    public class ElementModel : BaseModel
    {
        private int _id;
        public int ID
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(ID));
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private int _start = 0;
        public int Start
        {
            get => _start;
            set
            {
                _start = value;
                OnPropertyChanged(nameof(Start));
            }
        }

        private int _stop = 0;
        public int Stop
        {
            get => _stop;
            set
            {
                _stop = value;
                OnPropertyChanged(nameof(Stop));
            }
        }

        private int _ro1 = 0;
        public int RO1
        {
            get => _ro1;
            set
            {
                _ro1 = value;
                OnPropertyChanged(nameof(RO1));
            }
        }

        private int _ro2 = 0;
        public int RO2
        {
            get => _ro2;
            set
            {
                _ro2 = value;
                OnPropertyChanged(nameof(RO2));
            }
        }

        private float _cen = 0;
        public float CeN
        {
            get => _cen;
            set
            {
                _cen = value;
                OnPropertyChanged(nameof(CeN));
            }
        }

        public ElementModel() {}

        public ElementModel(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public ElementModel(int id, string name, int start, int stop, int ro1, int ro2, float cen)
        {
            ID = id;
            Name = name;
            Start = start;
            Stop = stop;
            RO1 = ro1;
            RO2 = ro2;
            CeN = cen;
        }
    }
}
