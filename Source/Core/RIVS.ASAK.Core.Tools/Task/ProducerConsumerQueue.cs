using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace RIVS.ASAK.Core.Tools.Task
{
    public class ProducerConsumerQueue<TArg> : IDisposable
    {
        private class ProducerConsumerWorkItem<TWArg>
        {
            public readonly TaskCompletionSource<object> TaskSource;
            public readonly object Work;
            public readonly TWArg Arg;
            public readonly CancellationToken? CancelToken;

            public ProducerConsumerWorkItem(
                TaskCompletionSource<object> taskSource,
                object work,
                CancellationToken? cancelToken = null)
            {
                TaskSource = taskSource ?? throw new ArgumentNullException(nameof(taskSource));
                Work = work ?? throw new ArgumentNullException(nameof(work));
                CancelToken = cancelToken;
            }
            public ProducerConsumerWorkItem(
                TaskCompletionSource<object> taskSource,
                object work,
                TWArg arg,
                CancellationToken? cancelToken = null)
            {
                TaskSource = taskSource ?? throw new ArgumentNullException(nameof(taskSource));
                Work = work ?? throw new ArgumentNullException(nameof(work));
                CancelToken = cancelToken;
                Arg = arg;
            }
        }

        private readonly BlockingCollection<ProducerConsumerWorkItem<TArg>> _taskQ = new BlockingCollection<ProducerConsumerWorkItem<TArg>>();

        public ProducerConsumerQueue(int workerCount, TaskCreationOptions creationOptions)
        {
            // Create and start a separate Task for each consumer:
            for (int i = 0; i < workerCount; i++)
                System.Threading.Tasks.Task.Factory.StartNew(Consume, creationOptions);
        }

        public System.Threading.Tasks.Task EnqueueTask(Action action, CancellationToken? cancelToken = null)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var tcs = new TaskCompletionSource<object>();
            _taskQ.Add(new ProducerConsumerWorkItem<TArg>(tcs, action, cancelToken));
            return tcs.Task;
        }

        public System.Threading.Tasks.Task EnqueueTask(Action<TArg> action, TArg arg, CancellationToken? cancelToken = null)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (arg == null) throw new ArgumentNullException(nameof(arg));

            var tcs = new TaskCompletionSource<object>();
            _taskQ.Add(new ProducerConsumerWorkItem<TArg>(tcs, action, arg, cancelToken));
            return tcs.Task;
        }

        public System.Threading.Tasks.Task EnqueueTask(Func<object> func, CancellationToken? cancelToken = null)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            var tcs = new TaskCompletionSource<object>();
            _taskQ.Add(new ProducerConsumerWorkItem<TArg>(tcs, func, cancelToken));
            return tcs.Task;
        }

        public System.Threading.Tasks.Task EnqueueTask(Func<object, object> func, TArg arg, CancellationToken? cancelToken = null)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            if (arg == null) throw new ArgumentNullException(nameof(arg));

            var tcs = new TaskCompletionSource<object>();
            _taskQ.Add(new ProducerConsumerWorkItem<TArg>(tcs, func, arg, cancelToken));
            return tcs.Task;
        }

        private void Consume()
        {
            foreach (var workItem in _taskQ.GetConsumingEnumerable())
                if (workItem.CancelToken.HasValue &&
                    workItem.CancelToken.Value.IsCancellationRequested)
                {
                    workItem.TaskSource.SetCanceled();
                }
                else
                    try
                    {
                        if (workItem.Work is Action actionVoid)
                        {
                            actionVoid();
                            workItem.TaskSource.SetResult(null);
                            continue;
                        }

                        if (workItem.Work is Action<TArg> actionArg1)
                        {
                            actionArg1(workItem.Arg);
                            workItem.TaskSource.SetResult(null);
                            continue;
                        }

                        if (workItem.Work is Func<object> funcVoid)
                        {
                            workItem.TaskSource.SetResult(funcVoid());
                            continue;
                        }

                        if (workItem.Work is Func<TArg, object> funcArg1)
                        {
                            workItem.TaskSource.SetResult(funcArg1(workItem.Arg));
                            continue;
                        }
                        throw new ArgumentOutOfRangeException($"EnqueueTask has unsupported type {workItem.Work.GetType()}");
                    }
                    catch (OperationCanceledException ex)
                    {
                        if (ex.CancellationToken == workItem.CancelToken)
                            workItem.TaskSource.SetCanceled();
                        else
                            workItem.TaskSource.SetException(ex);
                    }
                    catch (Exception ex)
                    {
                        workItem.TaskSource.SetException(ex);
                    }
        }

        #region IDisposable Support
        private bool _disposedValue; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты).
                    _taskQ.CompleteAdding();
                    //_taskQ.Dispose();
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.

                _disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~ScanTasksQueue() {
        //   // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
        //   Dispose(false);
        // }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
