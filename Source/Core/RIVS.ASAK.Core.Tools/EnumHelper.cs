using System;
using System.Collections.Generic;
using System.Linq;

namespace RIVS.ASAK.Core.Tools
{
    public class EnumHelper
    {
        public static TAttributeType GetAttribute<TAttributeType, TEnumType>(TEnumType e)
            where TAttributeType : Attribute
            where TEnumType : struct
        {
            if (!typeof(TEnumType).IsEnum)
                throw new ArgumentException("Only enum members are allowed");
            var vals = Enum.GetValues(typeof(TEnumType)).Cast<TEnumType>().ToList();
            var val = vals.Single(a => a.Equals(e));
            var member = typeof(TEnumType).GetMember(val.ToString()).Single();
            return member.GetCustomAttributes(typeof(TAttributeType), true).FirstOrDefault() as TAttributeType;
        }
        public static IEnumerable<Enum> GetIndividualFlags(Enum value)
        {
            return GetFlags(value, GetFlagValues(value.GetType()).ToArray());
        }

        private static IEnumerable<Enum> GetFlags(Enum value, Enum[] values)
        {
            ulong bits = System.Convert.ToUInt64(value);
            List<Enum> results = new List<Enum>();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                ulong mask = System.Convert.ToUInt64(values[i]);
                if (i == 0 && mask == 0L)
                    break;
                if ((bits & mask) == mask)
                {
                    results.Add(values[i]);
                    bits -= mask;
                }
            }
            if (bits != 0L)
                return Enumerable.Empty<Enum>();
            if (System.Convert.ToUInt64(value) != 0L)
                return results.Reverse<Enum>();
            if (bits == System.Convert.ToUInt64(value) && values.Length > 0 && System.Convert.ToUInt64(values[0]) == 0L)
                return values.Take(1);
            return Enumerable.Empty<Enum>();
        }

        private static IEnumerable<Enum> GetFlagValues(Type enumType)
        {
            ulong flag = 0x1;
            foreach (var value in Enum.GetValues(enumType).Cast<Enum>())
            {
                ulong bits = System.Convert.ToUInt64(value);
                if (bits == 0L)
                    continue;
                while (flag < bits) flag <<= 1;
                if (flag == bits)
                    yield return value;
            }
        }
    }
}
