//using System;
//using System.Windows.Threading;

//namespace RIVS.ASAK.UIElements.Tools.DispatcherTools
//{
//    public static class DispatcherHelper
//    {
//        /// <summary>
//        /// Выполнить сихронно в потоке диспатчера
//        /// </summary>
//        public static void RunSync(this System.Windows.Threading.Dispatcher dispatcher, Action function, DispatcherPriority priority = DispatcherPriority.Normal)
//        {
//            if (dispatcher.CheckAccess())
//            {
//                function();
//            }
//            else
//            {
//                dispatcher.Invoke(function, priority);
//            }
//        }

//        /// <summary>
//        /// Выполнить ассихронно в потоке диспатчера
//        /// </summary>
//        public static void RunAsync(this System.Windows.Threading.Dispatcher dispatcher, Action function, DispatcherPriority priority = DispatcherPriority.Normal)
//        {
//            dispatcher.BeginInvoke(function, priority);
//        }

//        /// <summary>
//        /// Выполнить синхронно (если мы в потоке диспатчера) или ассихронно
//        /// </summary>
//        public static void Run(this System.Windows.Threading.Dispatcher dispatcher, Action function, DispatcherPriority priority = DispatcherPriority.Normal)
//        {
//            if (dispatcher.CheckAccess())
//            {
//                function();
//            }
//            else
//            {
//                dispatcher.BeginInvoke(function, priority);
//            }
//        }
//    }
//}
