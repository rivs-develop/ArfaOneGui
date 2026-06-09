using System;
using System.Runtime.CompilerServices;

namespace RIVS.ASAK.Core.Contract
{
    public interface ILoggerFactory
    {
        ILogger Create(Type type = null, [CallerFilePath] string path = "");
    }
}
