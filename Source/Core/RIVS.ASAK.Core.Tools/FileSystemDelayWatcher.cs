using System.IO;
using System.Threading;

namespace RIVS.ASAK.Core.Tools
{
    /// <summary>
    /// Альтернатива системному FileSystemWatcher - попвещает об изменении файла не сразу, а с заданной задержкой
    /// </summary>
    public class FileSystemDelayWatcher : FileSystemWatcher
    {
        //задержка (в мс)
        private int _delay;

#region таймер, обеспечивающий задержку, и прочие данные, ему необходимые
        //таймер
        private Timer _delayTimer = null;
        //объект синхронизации доступа к иаймеру
        private object _delayTimerSynk = new object();
        //отправитель сообщения об изменении файла
        private object _sender;
        //аргумент события
        private FileSystemEventArgs _e;
#endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="path">папка с исследуемыми файлами</param>
        /// <param name="filter">файлы для мониторинга</param>
        /// <param name="delay">задержка сработки (мс)</param>
        public FileSystemDelayWatcher(string path, string filter, int delay)
            : base(path, filter)
        {
            _delay = delay;
            Changed += OnChangedMoment;
        }

        //системный FileSystemWatcher оповещает нас об изменении
        protected void OnChangedMoment(object sender, FileSystemEventArgs e)
        {
            lock(_delayTimerSynk)
            {
                if(_delayTimer != null)
                {
                    _delayTimer.Change(0, 0);
                    _delayTimer.Dispose();
                }
                _sender = sender;
                _e = e;
                _delayTimer = new Timer(OnTimer, this, _delay, 0);
            }
        }

        public event FileSystemEventHandler ChangedDelay;

        //пришло время оповестить клиентов
        protected void OnTimer(object This)
        {
            lock (_delayTimerSynk)
            {
                if (_delayTimer != null)
                {
                    if (ChangedDelay != null)
                    {
                        ChangedDelay.Invoke(_sender, _e);
                    }
                    _delayTimer.Dispose();
                    _delayTimer = null;
                }
                _e = null;
                _sender = null;
            }
        }
    }
}
