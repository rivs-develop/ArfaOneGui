using System.Collections.Concurrent;

namespace RIVS.ASAK.Core.Tools.ExtensionMethods
{
    public static class ConcurrentDictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(
            this ConcurrentDictionary<TKey, TValue> dict,
            TKey key,
            TValue defaultValue = default)
        {
            return dict.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }
}
