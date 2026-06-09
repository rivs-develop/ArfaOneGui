using System;
using System.Runtime.CompilerServices;
using RIVS.ASAK.Core.Contract;

namespace RIVS.ASAK.Core.Logging.Console
{
    public class ConsoleLoggerFactory : ILoggerFactory
    {
        public ILogger Create(Type type = null, [CallerFilePath] string path = "")
        {
            return new ConsoleLogger();
        }
    }
}
