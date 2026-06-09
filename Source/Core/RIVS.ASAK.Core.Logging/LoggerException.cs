using System;

namespace RIVS.ASAK.Core.Logging
{
    public class LoggerException : Exception
    {
        public LoggerException() { }
        public LoggerException(string msg) : base(msg) { }
        public LoggerException(string msg, Exception innerException) : base(msg, innerException)
        {

        }
    }
}
