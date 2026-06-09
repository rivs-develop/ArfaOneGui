using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace RIVS.ASAK.Core.Tools
{
    public class ConcurrentSortedDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        #region Variables

        private ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();
        SortedDictionary<TKey, TValue> _dict;

        #endregion

        #region Constructors

        public ConcurrentSortedDictionary()
        {
            _dict = new SortedDictionary<TKey, TValue>();
        }

        public ConcurrentSortedDictionary(IComparer<TKey> comparer)
        {
            _dict = new SortedDictionary<TKey, TValue>(comparer);
        }

        public ConcurrentSortedDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _dict = new SortedDictionary<TKey, TValue>(dictionary);
        }

        public ConcurrentSortedDictionary(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer)
        {
            _dict = new SortedDictionary<TKey, TValue>(dictionary, comparer);
        }

        #endregion

        #region Properties

        public IComparer<TKey> Comparer
        {
            get
            {
                _readWriteLock.EnterReadLock();
                try
                {
                    return _dict.Comparer;
                }
                finally
                {
                    _readWriteLock.ExitReadLock();
                }
            }
        }

        public int Count
        {
            get
            {
                _readWriteLock.EnterReadLock();
                try
                {
                    return _dict.Count;
                }
                finally
                {
                    _readWriteLock.ExitReadLock();
                }
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                _readWriteLock.EnterReadLock();
                try
                {
                    return _dict[key];
                }
                finally
                {
                    _readWriteLock.ExitReadLock();
                }
            }
            set
            {
                _readWriteLock.EnterWriteLock();
                try
                {
                    _dict[key] = value;
                }
                finally
                {
                    _readWriteLock.ExitWriteLock();
                }
            }
        }

        public SortedDictionary<TKey, TValue>.KeyCollection Keys
        {
            get
            {
                _readWriteLock.EnterReadLock();
                try
                {
                    return new SortedDictionary<TKey, TValue>.KeyCollection(_dict);
                }
                finally
                {
                    _readWriteLock.ExitReadLock();
                }
            }
        }

        public SortedDictionary<TKey, TValue>.ValueCollection Values
        {
            get
            {
                _readWriteLock.EnterReadLock();
                try
                {
                    return new SortedDictionary<TKey, TValue>.ValueCollection(_dict);
                }
                finally
                {
                    _readWriteLock.ExitReadLock();
                }
            }
        }

        #endregion

        #region Methods

        public void Add(TKey key, TValue value)
        {
            _readWriteLock.EnterWriteLock();
            try
            {
                _dict.Add(key, value);
            }
            finally
            {
                _readWriteLock.ExitWriteLock();
            }
        }

        public void Clear()
        {
            _readWriteLock.EnterWriteLock();
            try
            {
                _dict.Clear();
            }
            finally
            {
                _readWriteLock.ExitWriteLock();
            }
        }

        public bool ContainsKey(TKey key)
        {
            _readWriteLock.EnterReadLock();
            try
            {
                return _dict.ContainsKey(key);
            }
            finally
            {
                _readWriteLock.ExitReadLock();
            }
        }

        public bool ContainsValue(TValue value)
        {
            _readWriteLock.EnterReadLock();
            try
            {
                return _dict.ContainsValue(value);
            }
            finally
            {
                _readWriteLock.ExitReadLock();
            }
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            _readWriteLock.EnterReadLock();
            try
            {
                _dict.CopyTo(array, index);
            }
            finally
            {
                _readWriteLock.ExitReadLock();
            }
        }

        public override bool Equals(Object obj)
        {
            _readWriteLock.EnterReadLock();
            try
            {
                return _dict.Equals(obj);
            }
            finally
            {
                _readWriteLock.ExitReadLock();
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new SafeEnumerator<KeyValuePair<TKey, TValue>>(_dict.GetEnumerator(), _readWriteLock);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SafeEnumerator<KeyValuePair<TKey, TValue>>(_dict.GetEnumerator(), _readWriteLock);
        }

        public override int GetHashCode()
        {
            _readWriteLock.EnterReadLock();
            try
            {
                return _dict.GetHashCode();
            }
            finally
            {
                _readWriteLock.ExitReadLock();
            }
        }

        public bool Remove(TKey key)
        {
            _readWriteLock.EnterWriteLock();
            try
            {
                return _dict.Remove(key);
            }
            finally
            {
                _readWriteLock.ExitWriteLock();
            }
        }

        public override string ToString()
        {
            _readWriteLock.EnterReadLock();
            try
            {
                return _dict.ToString();
            }
            finally
            {
                _readWriteLock.ExitReadLock();
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            _readWriteLock.EnterReadLock();
            try
            {
                return _dict.TryGetValue(key, out value);
            }
            finally
            {
                _readWriteLock.ExitReadLock();
            }
        }

        #endregion
    }
}
