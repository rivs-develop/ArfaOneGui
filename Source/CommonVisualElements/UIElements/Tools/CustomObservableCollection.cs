using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace RIVS.ASAK.UIElements.Tools
{
    /// <summary> 
    /// Представляет динамическую коллекцию данных, которая предоставляет уведомления при добавлении,
    /// удалении элементов или обновлении всего списка. 
    /// </summary> 
    /// <typeparam name="T"></typeparam> 
    public class CustomObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
    {
        /// <summary>
        /// Класс аргументов события добавления элемента.
        /// </summary>
        public class BeforeAddItemEventArgs : CancelEventArgs
        {
            /// <summary>
            /// Добавляемый элемент.
            /// </summary>
            public T Item { get; private set; }
            /// <summary>
            /// Элементы для замены исходного добавляемого элемента.
            /// </summary>
            public IEnumerable<T> ReplaceItems { get; set; }
            /// <summary>
            /// Используется замена исходного элемента.
            /// </summary>
            public bool UseReplacement
            {
                get { return ReplaceItems != null; }
            }
            /// <summary>
            /// Элементы для добавления.
            /// </summary>
            public IEnumerable<T> ItemsForAdd
            {
                get
                {
                    List<T> res = new List<T>();
                    if (!Cancel)
                    {
                        if (UseReplacement)
                        {
                            res.AddRange(ReplaceItems);
                        }
                        else
                        {
                            res.Add(Item);
                        }
                    }
                    return res;
                }
            }
            /// <summary>
            /// Конструктор.
            /// </summary>
            /// <param name="item">Добавляемый элемент.</param>
            public BeforeAddItemEventArgs(T item)
            {
                Item = item;
                ReplaceItems = null;
            }
        }
        /// <summary>
        /// Событие перед добавление нового элемента в коллекцию.
        /// </summary>
        public event EventHandler<BeforeAddItemEventArgs> BeforeAddItem;

        // Перекрытие данного события - хак для поддержки range actions в CollectionView (иначе падает)
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        // удерживать нотификации OnCollectionChanged
        private bool _suppressNotification;

        /// <summary> 
        /// Конструктор.
        /// </summary> 
        public CustomObservableCollection()
        {
        }

        /// <summary> 
        /// Конструктор с инициализацией. 
        /// </summary> 
        /// <param name="collection">Новая коллекция.</param>
        public CustomObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        /// <summary> 
        /// Добавление списка элементов в коллекцию. 
        /// </summary> 
        /// <param name="collection">Добавляемый список.</param>
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return;
            }
            _suppressNotification = true;
            List<T> newItems = collection.SelectMany(OnBeforeAddItem).ToList();
            newItems.ForEach(Add);
            _suppressNotification = false;
            if (newItems.Any())
            {
                OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems));
            }
        }

        /// <summary> 
        /// Удаление списка элементов из коллекции. 
        /// </summary> 
        /// <param name="collection">Удаляемый список.</param>
        public void RemoveRange(IEnumerable<T> collection)
        {
            _suppressNotification = true;
            var items = collection.ToList().Where(Remove).ToList();
            _suppressNotification = false;
            if (items.Any())
            {
                OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items));
            }
        }

        /// <summary> 
        /// Удаление всех эементов удавлетворяющих правилу. 
        /// </summary> 
        /// <param name="predicate">Правило.</param>
        public void RemoveAll(Func<T, bool> predicate)
        {
            RemoveRange(Items.Where(predicate));
        }

        /// <summary> 
        /// Удаление первого эемента удавлетворяющего правилу. 
        /// </summary> 
        /// <param name="predicate">Правило.</param>
        public void Remove(Func<T, bool> predicate)
        {
            Remove(Items.FirstOrDefault(predicate));
        }

        /// <summary>
        /// Сброс коллекции с новой инициализацией.
        /// </summary>
        /// <param name="collection">Новая коллекция.</param>
        public void Reset(IEnumerable<T> collection)
        {
            _suppressNotification = true;
            Clear();
            foreach (var item in collection)
            {
                foreach (var newItem in OnBeforeAddItem(item))
                {
                    Add(newItem);
                }
            }
            _suppressNotification = false;
            OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Проверка через событие возможности вставки элемента.
        /// </summary>
        /// <returns>Возвращает элементы, кот. можно добавить.</returns>
        private IEnumerable<T> OnBeforeAddItem(T item)
        {
            if (BeforeAddItem != null)
            {
                var args = new BeforeAddItemEventArgs(item);
                BeforeAddItem(this, args);
                return args.ItemsForAdd;
            }
            return new[] { item };
        }

        public void Clear(bool suppressNotification)
        {
            _suppressNotification = suppressNotification;
            Clear();
            _suppressNotification = false;
        }

        protected virtual void OnCollectionChangedMultiItem(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler handlers = CollectionChanged;
            if (handlers != null)
            {
                foreach (NotifyCollectionChangedEventHandler handler in
                    handlers.GetInvocationList())
                {
                    if (handler.Target is CollectionView)
                    {
                        ((CollectionView)handler.Target).Refresh();
                    }
                    else
                    {
                        handler(this, e);
                    }
                }
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification)
            {
                base.OnCollectionChanged(e);
                if (CollectionChanged != null)
                {
                    CollectionChanged.Invoke(this, e);
                }
            }
        }

        public void Sort(Comparison<T> comparison)
        {
            var tmp = Items.ToList();
            tmp.Sort(comparison);
            Reset(tmp);
        }

        public void Sort(IComparer<T> comparer)
        {
            var tmp = Items.OrderBy(i => i, comparer).ToList();
            Reset(tmp);
        }
    }
}
