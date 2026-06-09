using System.Collections.Generic;

namespace RIVS.ASAK.ARFA.Application
{
    public class UpdateGraphData
    {
        public UpdateGraphData(IEnumerable<ArfaGraphElement> arfaGraphElement, uint[] buffer)
        {
            ArfaGraphElement = arfaGraphElement;
            Buffer = buffer;
        }

        public IEnumerable<ArfaGraphElement> ArfaGraphElement { get; private set; }

        /// <summary>
        /// Буфер с данными
        /// </summary>
        public uint[] Buffer { get; private set; }
    }
}
