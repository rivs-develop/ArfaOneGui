using System;

namespace RIVS.ASAK.Core.Contract
{
    public interface ILogger
    {
        /// <summary>
        ///     trace
        /// </summary>
        /// <param name="info"></param>
        void Trace(string info);

        /// <summary>
        ///     general debug info
        /// </summary>
        /// <param name="info"></param>
        void Debug(string info);

        /// <summary>
        ///     general info
        /// </summary>
        /// <param name="info"></param>
        void Info(string info);

        /// <summary>
        ///   warning info
        /// </summary>
        /// <param name="info"></param>
        void Warn(string info);

        /// <summary>
        ///     exception or error log
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ex"></param>
        void Error(string info, Exception ex);

        /// <summary>
        /// error
        /// </summary>
        /// <param name="info"></param>
        void Error(string info);

        /// <summary>
        /// exception
        /// </summary>
        /// <param name="ex"></param>
        void Error(Exception ex);
    }
}