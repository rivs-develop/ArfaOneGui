using System.Collections.Generic;
using Prism.Events;

namespace RIVS.ASAK.ARFA.GUI.EventArgs
{
    public class HighlightCuvetteChangedEvent : PubSubEvent<HighlightCuvetteChangedEventArgs>
    {
    }

    public class HighlightCuvetteChangedEventArgs
    {
        public List<int> HighlightCuvette { get; private set; }

        public HighlightCuvetteChangedEventArgs(List<int> arrHighlightCuvette)
        {
            HighlightCuvette = arrHighlightCuvette;
        }
    }
}
