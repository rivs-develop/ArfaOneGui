using System;

namespace RIVS.ASAK.Core.Logging
{
    [Flags]
    public enum LogLevel
    {
        Debug = 2,
        Info = 4,
        Warn = 8,
        Error = 16,
        Trace = 32
    }
}
