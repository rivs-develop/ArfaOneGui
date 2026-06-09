using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIVS.ASAK.Core.Contract.DTO;

namespace RIVS.ASAK.ARFA.Application
{
    public class ElementJaItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Ja { get; set; }
    }

    public class ReperDataItem
    {
        public DateTime MeasDt { get; set; }
        public int Pribor { get; set; }
        public int Cuvet { get; set; }
        public int ReperHashKey { get; set; }
        public IEnumerable<ElementJaItem> ElementJaItems { get; set; }

        public ReperDataItem(DateTime measDt, int pribor, int cuvet, int reperHashKey,
            IEnumerable<ElementJaModel> elementJaItems)
        {
            MeasDt = measDt;
            Pribor = pribor;
            Cuvet = cuvet;
            ReperHashKey = reperHashKey;

            var newResult = elementJaItems.Select(item => new ElementJaItem() { Id = item.Id, Name = item.Name, Ja = item.Ja }).ToList();
            ElementJaItems = newResult;
        }

        public ReperDataItem()
        {
        }

        public override string ToString()
        {
            var stb = new StringBuilder();
            stb.Append($"MeasDt={MeasDt}, Pribor={Pribor}, Cuvet={Cuvet}, ReperHashKey={ReperHashKey} ");
            foreach (var item in ElementJaItems)
            {
                stb.Append($"Id={item.Id}, Name={item.Name}, Ja={item.Ja} ");
            }
            return stb.ToString();
        }
    }

    /// <summary>
    /// Кольцевой буфер данных по реперам
    /// </summary>
    public class RepersDataQueue
    {
        private readonly LinkedList<ReperDataItem> _queue;
        private readonly int _queueMaxCount;

        public RepersDataQueue(int size)
        {
            _queue = new LinkedList<ReperDataItem>();
            _queueMaxCount = size;
        }

        public void Enqueue(ReperDataItem obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            lock (_queue)
            {
                while (_queue.Count > _queueMaxCount - 1)
                {
                    _queue.RemoveFirst();
                }
                _queue.AddLast(obj);
            }
        }

        public ReperDataItem GetFirstItem()
        {
            lock (_queue)
            {
                if (0 != _queue.Count)
                {
                    return _queue.First.Value;
                }
            }
            return null;
        }

        public ReperDataItem GetLastItem()
        {
            lock (_queue)
            {
                if (0 != _queue.Count)
                {
                    return _queue.Last.Value;
                }
            }
            return null;
        }

        public void Clear()
        {
            lock (_queue)
            {
                _queue.Clear();
            }
        }

        public int Count
        {
            get
            {
                lock (_queue)
                {
                    return _queue.Count;
                }
            }
        }
    }
}

