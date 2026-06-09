using System.Collections;
using System.Collections.Generic;

namespace RIVS.ASAK.Core.Contract.Values
{
    public interface INamedCollection<T> : ICollection<T>, ICollection
        where T : class, INamed
    {
        T this[string name] { get; }

        bool Contains(string name);
        T Get(string name);

        bool Set(T item);
        bool Set(INamedCollection<T> collection);
    }
}
