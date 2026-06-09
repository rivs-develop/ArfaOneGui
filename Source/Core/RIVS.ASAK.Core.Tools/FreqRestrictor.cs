using System;

namespace RIVS.ASAK.Core.Tools
{
    /// <summary>
    /// Ограничитель частоты каких-либо действий
    /// Ограничение по времени и/или по частоте
    /// </summary>
    public class FreqRestrictor
    {
        /// <summary>
        /// ограничение по частоте действий 
        /// </summary>
        public int CountFreq { get; set; }
        /// <summary>
        /// ограничение по времени действий
        /// </summary>
        public TimeSpan TimeFreq     { get; set; }

        public int SkipActionCount { get; private set; }
        public int LastSkipActionCount { get; private set; }
        public DateTime LastActionTime { get; private set; }


        public FreqRestrictor(TimeSpan timeFreq, int countFreq)
        {
            TimeFreq = timeFreq;
            CountFreq = countFreq;
            SkipActionCount = 0;
            LastSkipActionCount = 0;
            LastActionTime = DateTime.Now;
        }

        public FreqRestrictor(int countFreq)
            : this(TimeSpan.MaxValue, countFreq)
        {
        }

        public FreqRestrictor(TimeSpan timeFreq)
            : this(timeFreq, int.MaxValue)
        {
        }

        public bool IsActionAllowed()
        {
            if ((DateTime.Now - LastActionTime > TimeFreq) || 
                (SkipActionCount + 1 > CountFreq))
            {
                LastSkipActionCount = SkipActionCount;
                SkipActionCount = 0;
                LastActionTime = DateTime.Now;
                return true;
            }
            else
            {
                SkipActionCount++;
                return false;
            }
        }
    }
}
