using System;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;

namespace RIVS.ASAK.Core.Tools.Serialization
{
    public class ConfigHelper
    {
        public static void SetField<T>(XObject source, T destination, string destinationFieldName)
        {
            var sourceType = source.GetType();
            var sourcePropertyHandle = sourceType.GetProperty("Value");
            Debug.Assert(sourcePropertyHandle != null, "Передали некорректный Xml.Linq тип. Не содержит свойство Value.");
            if (sourcePropertyHandle == null)
                throw new ArgumentException("Передали некорректный Xml.Linq тип. Не содержит свойство Value.");
            var sourceValue =
                (string)source.GetType().InvokeMember("Value", BindingFlags.GetProperty, null, source, new object[] { });

            var destinationType = typeof(T);
            var destinationFieldHandle = destinationType.GetField(destinationFieldName);
            if (destinationFieldHandle == null)
                throw new ArgumentException(String.Format("Класс: {0} не содержит поля: {1}", destinationType, destinationFieldName));
            var destinationFieldType = destinationFieldHandle.FieldType;

            object fieldValue = sourceValue;
            if (destinationFieldType != typeof(String))
            {
                var parseMethodHandle = destinationFieldType.GetMethod("Parse", new[] { typeof(String) });

                // Если метод Parse не найден
                if (parseMethodHandle == null)
                {
                    // Тип Enum, то
                    if (destinationFieldType.IsEnum)
                    {
                        fieldValue = Enum.Parse(destinationFieldType, sourceValue);
                    }
                    else
                    {
                        throw new ArgumentException(String.Format("Невозможно прочитать поле: {0} класса: {1}. В типе {2} отсутствует метод Parse()",
                            destinationFieldName, destinationType, destinationFieldType.Name));
                        
                    }
                }
                else
                {

                    fieldValue = destinationFieldType.InvokeMember("Parse", BindingFlags.InvokeMethod, null,
                                                                   destinationFieldType,
                                                                   new object[] { sourceValue });
                }
            }
            destinationFieldHandle.SetValue(destination, Convert.ChangeType(fieldValue, destinationFieldType));
        }
    }
}
