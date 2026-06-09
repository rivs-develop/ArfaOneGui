using System;
using System.Collections.Generic;
using System.Linq;

namespace RIVS.ASAK.Core.Tools.ExtensionMethods
{
    /// <summary>
    /// Расширение для List.
    /// </summary>
    public static class ListExtension
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }
}
