using System;

namespace RIVS.ASAK.Core.Logging
{
    static class Extensions
    {
        public static string ToStackTraceString(this Exception exception)
        {
            if (exception == null)
                return string.Empty;
            return GetExceptionMessage(exception);
        }

        private static string GetExceptionMessage(Exception exception)
        {
            if (exception == null)
                return string.Empty;

            var message = $"{exception.Message} StackTrace:{exception.StackTrace}";
            if (exception.InnerException != null)
                message += "|InnerException:" + GetExceptionMessage(exception.InnerException);
            return message;
        }
    }
}
