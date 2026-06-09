using System;
using System.Windows.Threading;

namespace RIVS.ASAK.ARFA.GUI.Helpers
{
    public static class DispatcherHelper
    {
        internal static System.Windows.Threading.Dispatcher InterfaceLevelDispatcher { get; set; }

        /// <summary>
        /// Выполнить сихронно в потоке диспатчера
        /// </summary>
        public static void RunSync(Action function, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            if (InterfaceLevelDispatcher.CheckAccess())
            {
                function();
            }
            else
            {
                InterfaceLevelDispatcher.Invoke(function, priority);
            }
        }

        /// <summary>
        /// Выполнить ассихронно в потоке диспатчера
        /// </summary>
        public static void RunAsync(Action function, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            InterfaceLevelDispatcher.BeginInvoke(function, priority);
        }

        /// <summary>
        /// Выполнить синхронно (если мы в потоке диспатчера) или ассихронно
        /// </summary>
        public static void Run(Action function, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            if (InterfaceLevelDispatcher.CheckAccess())
            {
                function();
            }
            else
            {
                InterfaceLevelDispatcher.BeginInvoke(function, priority);
            }
        }
    }
}
