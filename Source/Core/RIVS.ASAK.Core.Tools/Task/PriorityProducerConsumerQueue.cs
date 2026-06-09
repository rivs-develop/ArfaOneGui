using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RIVS.ASAK.Core.Tools.Task
{
    public class PriorityQueue2<T> where T : IComparable<T>
    {
        private const int DEFAULT_CAPACITY = 10;
        private T[] items;
        private int size = 0;

        private static T[] emptyArray = new T[0];

        public int Size { get { return size; } }

        public PriorityQueue2()
        {
            items = emptyArray;
        }

        public PriorityQueue2(int size)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException("Size must be 0 or greater");

            items = new T[size];
        }

        public PriorityQueue2(IEnumerable<T> sourceCollection)
        {
            if (sourceCollection == null)
                throw new ArgumentNullException("sourceCollection");

            var sourceSize = sourceCollection.Count();
            items = new T[sourceSize];
            for (int i = 0; i < sourceSize; i++)
            {
                InsertAndShift(sourceCollection.ElementAt(i));
            }
            size = sourceSize;
        }

        public void Clear()
        {
            items = emptyArray;
            size = 0;
        }

        public void Insert(T item)
        {
            InsertAndShift(item);
        }

        public void InsertRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            var requiredSize = size + collection.Count();
            if (requiredSize > items.Length)
                Enlarge(requiredSize);

            foreach (var item in collection)
            {
                InsertAndShift(item);
            }
        }

        private void InsertAndShift(T item)
        {
            if (size == items.Length)
                Enlarge();

            var index = size++;
            items[index] = item;
            ShiftUp(index);
        }

        private void ShiftUp(int index)
        {
            var parent = GetParent(index);
            while (index > 0 && items[index].CompareTo(items[parent]) >= 0)
            {
                Swap(index, parent);
                index = parent;
                parent = GetParent(index);
            }
        }

        private int GetParent(int index)
        {
            var parent = (index - 1) >> 1;
            return parent;
        }

        public T PeekTopItem()
        {
            if (size == 0)
                throw new InvalidOperationException("Empty queue");

            return items[0];
        }

        public T ExtractTopItem()
        {
            var topItem = PeekTopItem();

            items[0] = items[--size];

            ShiftDown();

            if (size <= items.Length >> 1)
                Reduce();

            return topItem;
        }

        private void ShiftDown()
        {
            var root = 0;

            while (root * 2 + 1 < size)
            {
                int next;
                var left = root * 2 + 1;

                if (items[left].CompareTo(items[root]) > 0)
                    next = left;
                else
                    next = root;

                var right = root * 2 + 2;
                if (right < size && items[right].CompareTo(items[next]) > 0)
                    next = right;
                if (next != root)
                {
                    Swap(next, root);
                    root = next;
                }
                else
                    break;
            }
        }

        private void Reduce()
        {
            if (size >= DEFAULT_CAPACITY)
            {
                var newSize = items.Length >> 1;
                Resize(newSize, false);
            }
        }

        private void Enlarge()
        {
            var newSize = size == 0 ? DEFAULT_CAPACITY : size << 1;
            Resize(newSize, true);
        }

        private void Enlarge(int addToSize)
        {
            var newSize = size + addToSize;
            Resize(newSize, true);
        }

        private void Resize(int newSize, bool enlarge)
        {
            var newItems = new T[newSize];
            Array.Copy(items, 0, newItems, 0, enlarge ? size : newSize);
            items = newItems;
        }

        private void Swap(int first, int second)
        {
            var temp = items[first];
            items[first] = items[second];
            items[second] = temp;
        }
    }

    public enum Priority
        {
        //Idle,
        BelowNormal =0,
            Normal=1,
            AboveNormal=2,
            //TimeCritical
        }

    public class ProducerConsumerQueue
        {
            private bool isShutdown = false;
            private readonly object _lock = new object();
            private readonly CancellationTokenSource cancelToken;
            private readonly PriorityQueue2<Worker> queue = new PriorityQueue2<Worker>();
            private readonly AutoResetEvent waitHandle = new AutoResetEvent(false);

            public ProducerConsumerQueue()
            {
                cancelToken = new CancellationTokenSource();
                System.Threading.Tasks.Task.Factory.StartNew(new Action(DoWork), TaskCreationOptions.LongRunning);
            }

            public System.Threading.Tasks.Task Enqueue(Action _delegate, Priority priority)
            {
                if (isShutdown)
                    throw new InvalidOperationException("Queue is shutdown");

                var worker = new Worker(_delegate, priority);
                lock (_lock)
                {
                    queue.Insert(worker);
                }
                waitHandle.Set();
                return worker.TaskCompletionSrc.Task;
            }

            public System.Threading.Tasks.Task Enqueue(Action _delegate)
            {
                return Enqueue(_delegate, Priority.Normal);
            }

            private void DoWork()
            {
                while (true)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        break;
                    }

                    Worker worker = null;
                    lock (_lock)
                    {
                        if (queue.Size > 0)
                        {
                            worker = queue.ExtractTopItem();
                        }
                    }

                    if (worker != null)
                    {
                        try
                        {
                            worker.Delegate.Invoke();
                            worker.TaskCompletionSrc.SetResult(null);
                        }
                        catch (Exception fail)
                        {
                            worker.TaskCompletionSrc.SetException(fail);
                        }
                    }
                    else
                    {
                        waitHandle.WaitOne();
                    }
                }
            }

            public void Shutdown()
            {
                isShutdown = true;
                cancelToken.Cancel();
                waitHandle.Set();
            }

            private class Worker : IComparable<Worker>
            {
                public Priority Priority { get; private set; }
                public TaskCompletionSource<object> TaskCompletionSrc { get; private set; }
                public Action Delegate { get; private set; }

                public Worker(Action _delegate, Priority priority)
                {
                    if (_delegate == null)
                        throw new ArgumentNullException("delegate");

                    this.Delegate = _delegate;
                    this.Priority = priority;
                    this.TaskCompletionSrc = new TaskCompletionSource<object>();
                }

                public int CompareTo(Worker other)
                {
                    if (this.Priority > other.Priority)
                        return 1;
                    else if (this.Priority < other.Priority)
                        return -1;
                    return 0;
                }
            }
        }
    
}
