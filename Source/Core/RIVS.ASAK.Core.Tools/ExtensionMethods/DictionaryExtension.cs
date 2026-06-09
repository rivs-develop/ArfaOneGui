using System;
using System.Collections.Generic;

namespace RIVS.ASAK.Core.Tools.ExtensionMethods
{
    /// <summary>
    /// Расширение класса Dictionary
    /// </summary>
    public static class DictionaryExtension
    {
        /// <summary>
        /// Копировать словарь.
        /// Предназначен для случаев, когда содержимое - значимый тип или клонирование не требуется.
        /// Если же TValue поддерживает клонирование, то оно будет вызвано
        /// </summary>
        /// <typeparam name="TKey">Тип ключа</typeparam>
        /// <typeparam name="TValue">Тип значения</typeparam>
        /// <param name="original">Исходный словарь</param>
        /// <returns>Клонированый словарь</returns>
        public static Dictionary<TKey, TValue> CloneDictionary<TKey, TValue>(this Dictionary<TKey, TValue> original)
        {
            var ret = new Dictionary<TKey, TValue>(original.Count, original.Comparer);
            foreach (var entry in original)
            {
                var cloneable = entry.Value as ICloneable;
                if (cloneable != null)
                {
                    ret.Add(entry.Key, (TValue)cloneable.Clone());
                }
                else
                {
                    ret.Add(entry.Key, entry.Value);
                }
            }
            return ret;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defaultValue = default(TValue))
        {
            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }
}
