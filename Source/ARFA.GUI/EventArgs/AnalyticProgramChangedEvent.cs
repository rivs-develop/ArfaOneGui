using System;
using Prism.Events;

namespace RIVS.ASAK.ARFA.GUI.EventArgs
{

    public class AnalyticalProgramChangedEvent : PubSubEvent<AnalyticalProgramChangedEventArgs>
    {
    }

    public class AnalyticalProgramChangedEventArgs
    {
        public bool analyticalProjectsOn { get; private set; }
        public string Name { get; private set; }
        public DateTime dtActived { get; private set; }

        public AnalyticalProgramChangedEventArgs(bool state, string name, DateTime dt)
        {
            analyticalProjectsOn = state;
            Name = name;
            dtActived = dt;
        }
        public AnalyticalProgramChangedEventArgs()
        :this(false, string.Empty, DateTime.MinValue)
        {
        }
    }
}
