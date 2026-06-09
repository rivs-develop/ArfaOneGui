using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RIVS.ASAK.Core.Contract.Values
{
    public abstract class NamedCollection<T> : INamedCollection<T>
      where T : class, INamed
    {
        protected List<T> _collection = new List<T>();

        public NamedCollection() { }

        public NamedCollection(IEnumerable<T> collection)
        {
            if (collection != null)
            {
                _collection = collection.ToList();
            }
        }

        public T this[int index]
        { get { return _collection[index]; } }

        public T this[string name]
        { get { return Get(name); } }

        public T Get(string name)
        { return _collection.FirstOrDefault(e => e.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase)); }

        public T Get(int index)
        { return _collection[index]; }

        public virtual void Add(T item)
        {
            if (item == null)
            {
                return;
            }

            _collection.Add(item);
        }

        public virtual bool Remove(T item)
        {
            return _collection.Remove(item);
        }

        public virtual void Clear()
        {
            _collection.Clear();
        }

        public abstract bool Set(T item);

        public virtual bool Set(INamedCollection<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentException("Невозможно установить пустую коллекцию!");
            }

            var result = false;

            // обновляем элементы из новой коллекции
            foreach (var item in collection)
            {
                result = Set(item) || result;
            }

            // удаляем лишние элементы из текущей коллекции
            for (var i = Count - 1; i >= 0; i--)
            {
                var current = this[i];

                var exist = collection.Get(current.Name);
                if (exist == null)
                {
                    Remove(current);
                    result = true;
                }
            }

            return result;
        }

        public int Count
        { get { return _collection.Count; } }

        public bool IsReadOnly
        { get { return false; } }

        public object SyncRoot
        { get { return ((ICollection)_collection).SyncRoot; } }

        public bool IsSynchronized
        { get { return ((ICollection)_collection).IsSynchronized; } }

        public bool Contains(string name)
        { return Get(name) != null; }

        public bool Contains(T item)
        {
            return _collection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _collection.CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_collection).CopyTo(array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    //public abstract class NamedOwneredCollection<T> : NamedCollection<T>, IOwnered
    //    where T : class, INamed, IOwnered
    //{
    //    protected Owner _owner = null;

    //    public Owner Owner { get { return _owner; } }

    //    public NamedOwneredCollection() { }

    //    public NamedOwneredCollection(Owner owner)
    //    { _owner = owner; }

    //    public NamedOwneredCollection(Owner owner, IEnumerable<T> collection)
    //    {
    //        _owner = owner;
    //        if (collection != null)
    //            _collection = collection.ToList();
    //    }

    //    public override void Add(T item)
    //    {
    //        if (item == null)
    //            return;

    //        if (!item.HasOwner() && this.Owner != null)
    //            item.ChangeOwner(this.Owner);

    //        _collection.Add(item);
    //    }

    //    public bool HasOwner()
    //    {
    //        return this.Owner != null && (this.Owner.ID > 0);
    //    }

    //    public virtual void ChangeOwner(Owner owner)
    //    {
    //        if (owner == null)
    //            throw new CustomArgumentException("Не задан владелец коллекции для обновления!");

    //        if (owner == this.Owner)
    //            return;

    //        if (this.HasOwner())
    //            throw new CustomOperationException($"У коллекции уже задан другой владелец ({this.Owner.ToString()}).");

    //        _owner = owner;
    //    }

    //    public void ChangeOwner(IOwner owner)
    //    {
    //        this.ChangeOwner(owner?.GetOwner());
    //    }
    //}
}
