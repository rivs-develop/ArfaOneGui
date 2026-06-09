using System;
using RIVS.ASAK.Core.Contract;

namespace RIVS.ASAK.Core.Logging.Console
{
    /// <summary>
    ///     echo log throgh console
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        public ConsoleLogger()
        {

        }

        public void Trace(string info)
        {
            System.Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} TRACE {info}");
        }

        public void Debug(string info)
        {
            System.Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} DEBUG {info}");
        }

        public void Error(string info, Exception ex)
        {
            System.Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} ERROR {info},{ex.ToStackTraceString()}");
        }

        public void Error(string info)
        {
            System.Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} ERROR {info}");
        }

        public void Error(Exception ex)
        {
            System.Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} ERROR {ex.ToStackTraceString()}");
        }

        public void Info(string info)
        {
            System.Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} INFO {info}");
        }

        public void Warn(string info)
        {
            System.Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} WARN {info}");
        }
    }
}