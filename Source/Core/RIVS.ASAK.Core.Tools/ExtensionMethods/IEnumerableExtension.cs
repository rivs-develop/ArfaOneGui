using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RIVS.ASAK.Core.Tools.ExtensionMethods
{
    /// <summary>
    /// Расширение для типа IEnumerable.
    /// </summary>
    public static class IEnumerableExtension
    {
        /// <summary>
        /// Преобразование из списка в словарь.
        /// Отличие от обычного ToDictionary:
        /// - на вход селекторы ключа подается ВСЯ коллекция и значение КЛЮЧА для которого формируется значение. 
        /// </summary>
        /// <typeparam name="T">Тип элемента коллекции</typeparam>
        /// <typeparam name="TKey">Тип ключа</typeparam>
        /// <typeparam name="TValue">Тип значения</typeparam>
        /// <param name="list">Список элементов из которых формируется коллекция</param>
        /// <param name="keySelector">Функция селектор ключа (на входе: элемент коллекции, на выходе: значение являющееся ключом)</param>
        /// <param name="valueSelector">Функция селектора значения (на входе: коллекция элементов, значение ключа для которого формируется значение,
        /// на выходе: значение для указанного ключа</param>
        /// <returns>Сформированный словарь</returns>
        public static Dictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(this IEnumerable<T> list, Func<T, TKey> keySelector, Func<IEnumerable<T>, TKey, TValue> valueSelector)
        {
            Debug.Assert(list != null, "list == null");
            Debug.Assert(keySelector != null, "keySelector == null");
            Debug.Assert(valueSelector != null, "valueSelector == null");

            return list.Select(keySelector).Distinct().Select(a => new
                {
                    Key = a,
                    Value = valueSelector(list, a)
                }).ToDictionary(a => a.Key, a => a.Value);
        }
        /// <summary>
        /// Аналог List.ForEach().
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="list">Коллекция, для каждого элемента которой вызывается action</param>
        /// <param name="action">Делегат, вызываемый для каждого элемента коллекции list</param>
        public static void ForEach<TValue>(this IEnumerable<TValue> list, Action<TValue> action)
        {
            foreach (var value in list)
            {
                action(value);
            }
        }
        /// <summary>
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="list"></param>
        /// <param name="delimiter"></param>
        /// <param name="maxLength"></param>
        /// <param name="cropString"></param>
        /// <returns></returns>
        public static string ToString<TValue>(this IEnumerable<TValue> list, int maxLength = 255, string delimiter = ", ", string cropString = ", ...")
        {
            int maxPostfixLength = delimiter.Length > cropString.Length ? delimiter.Length : cropString.Length;

            bool isCropped = false;
            var result = string.Empty;
            foreach(var value in list)
            {
                if (result.Length + value.ToString().Length + maxPostfixLength < maxLength)
                {
                    result += value.ToString() + delimiter;
                    continue;
                }
                result += cropString;
                isCropped = true;
                break;
            }

            if (!isCropped)
            {
                //удалим последний delimiter
                result = result.Remove(result.Length - delimiter.Length, delimiter.Length);
            }
            return result;
        }

        /// <summary>
        /// Проверка на отсутствие элементов в коллекции.
        /// </summary>
        /// <typeparam name="TValue">Тип элемента коллекции</typeparam>
        /// <param name="list">Коллекци, для которой выполняется проверка</param>
        /// <returns></returns>
        public static bool Empty<TValue>(this IEnumerable<TValue> list)
        {
            return (list == null) || !list.Any();
        }

        /// <summary>
        /// Поиск элемента с минимальным значением какого-либо свойства.
        /// </summary>
        /// <typeparam name="TValue">Тип элемента коллекции</typeparam>
        /// <typeparam name="TProperty">Тип свойства сравнимаемого элемента коллекции</typeparam>
        /// <param name="list"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static TValue ExtMin<TValue, TProperty>(this IEnumerable<TValue> list, Func<TValue, TProperty> selector) where TProperty : IComparable 
        {
            if (list == null)
                throw new ArgumentNullException("list");
            if (selector == null)
                throw new ArgumentNullException("selector");

            var minEl = list.ElementAt(0);
            var minVal = selector(minEl);
            foreach (var value in list.Skip(1))
            {
                var curVal = selector(value);
                if (curVal.CompareTo(minVal) > 0)
                    continue;
                minEl = value;
                minVal = curVal;
            }
            return minEl;
        }
    }
}
