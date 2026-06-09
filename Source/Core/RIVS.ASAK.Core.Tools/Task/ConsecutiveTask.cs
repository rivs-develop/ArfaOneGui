using System;
using System.Threading;
using System.Threading.Tasks;

namespace RIVS.ASAK.Core.Tools.Task
{
    /// <summary>
    /// Планировщик, обеспечивающий последовательное выполнение задач
    /// </summary>
    public class ConsecutiveTask
    {
        #region Private members

        /// <summary>
        /// Последняя выполненная задача
        /// </summary>
        private System.Threading.Tasks.Task _task;

        /// <summary>
        /// Планировщик задач
        /// </summary>
        private readonly TaskScheduler _scheduler;

        /// <summary>
        /// Сигнал отмены выполнения задач
        /// </summary>
        private CancellationTokenSource _cancellationToken = new CancellationTokenSource();

        /// <summary>
        /// Синхронизация запуска задач
        /// </summary>
        private readonly object _locker = new object();

        /// <summary>
        /// Свойства запускаемой задачи 
        /// </summary>
        private readonly TaskCreationOptions _creationOptions;

        /// <summary>
        /// Свойства Continuation задачи 
        /// </summary>
        private readonly TaskContinuationOptions _continuationOptions;

        #endregion

        /// <summary>
        /// Создаем экземпляр класса.
        /// Настройки по-умолчанию предполагают максимально быстрое выполению задач 
        /// </summary>
        public ConsecutiveTask()
            : this( // TaskCreationOptions.None, TaskContinuationOptions.None)
            new ConcurrentExclusiveSchedulerPair().ExclusiveScheduler,
            TaskCreationOptions.PreferFairness | TaskCreationOptions.LongRunning,
            TaskContinuationOptions.PreferFairness | TaskContinuationOptions.LongRunning)
        {
        }

        /// <summary>
        /// Создаем экземпляр класса.
        /// Задаем параметры выполнения задач.
        /// </summary>
        /// <param name="scheduler">Используемый планировщик</param>
        /// <param name="creationOptions">Параметры первой задачи</param>
        /// <param name="continuationOptions">Параметры последующих задач</param>
        public ConsecutiveTask(TaskScheduler scheduler, TaskCreationOptions creationOptions, TaskContinuationOptions continuationOptions)
        {
            _scheduler = scheduler;
            _creationOptions = creationOptions;
            _continuationOptions = continuationOptions;
        }

        public ConsecutiveTask(TaskScheduler scheduler)
            : this(
                scheduler,
                TaskCreationOptions.PreferFairness | TaskCreationOptions.LongRunning,
                TaskContinuationOptions.PreferFairness | TaskContinuationOptions.LongRunning)
        {
        }

        /// <summary>
        /// Запланировать задачу.
        /// Задачи выполняются строго последовательно.
        /// </summary>
        /// <param name="action">задача</param>
        public void Run(Action action)
        {
            lock(_locker)
            {
                _task = _task == null
                            ? System.Threading.Tasks.Task.Factory.StartNew(action,
                                                                           _cancellationToken.Token,
                                                                           _creationOptions,
                                                                           _scheduler)
                            : _task.ContinueWith(task => action(),
                                                 _cancellationToken.Token,
                                                 _continuationOptions,
                                                 _scheduler);
            }
        }

        /// <summary>
        /// Запустить задачу только если предыдущая уже завершена
        /// </summary>
        /// <param name="action">Задач для запуска</param>
        /// <returns>True, если задача запущена</returns>
        public bool RunIfCompleted(Action action)
        {
            lock (_locker)
            {
                if (IsCompleted)
                {
                    Run(action);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Отказаться от всех запланированных и будущих задач
        /// </summary>
        public void CancelAll()
        {
            _cancellationToken.Cancel();
        }

        /// <summary>
        /// Отказаться от всех старых задач и создать новый CancellationToken
        /// </summary>
        public void CancelOld()
        {
            _cancellationToken.Cancel();
            _cancellationToken = new CancellationTokenSource();
        }

        public CancellationToken Token
        {
            get
            {
                return _cancellationToken.Token;
            }
        }

        /// <summary>
        /// Возвращает true, если текущая задача завершена
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                lock (_locker)
                {
                    if (_task == null)
                    {
                        return true;
                    }

                    return _task.GetAwaiter().IsCompleted;
                }
        }
    }

        public void Wait()
        {
            if (IsCompleted)
            {
                return;
            }

            _task.Wait();
        }

        public bool Wait(int milliseconds)
        {
            return IsCompleted || _task.Wait(milliseconds);
        }

        public bool WaitWithCancel(int milliseconds)
        {
            return IsCompleted || _task.Wait(milliseconds, Token);
        }
    }
}
