using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace RIVS.ASAK.Core.Tools
{
  
    public class SafeEnumerator<T> : IEnumerator<T>
    {
        #region Variables

        // this is the (thread-unsafe)
        // enumerator of the underlying collection
        private readonly IEnumerator<T> _enumerator;

        // this is the object we shall lock on. 
        private ReaderWriterLockSlim _lock;

        #endregion

        #region Constructor

        public SafeEnumerator(IEnumerator<T> inner, ReaderWriterLockSlim readWriteLock)
        {
            _enumerator = inner;
            _lock = readWriteLock;

            // Enter lock in constructor
            _lock.EnterReadLock();
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            // .. and exiting lock on Dispose()
            // This will be called when the foreach loop finishes
            _lock.ExitReadLock();
        }

        #endregion

        #region Implementation of IEnumerator

        // we just delegate actual implementation
        // to the inner enumerator, that actually iterates
        // over some collection

        public bool MoveNext()
        {
            return _enumerator.MoveNext();
        }

        public void Reset()
        {
            _enumerator.Reset();
        }

        public T Current
        {
            get
            {
                return _enumerator.Current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        #endregion
    }

    
}
