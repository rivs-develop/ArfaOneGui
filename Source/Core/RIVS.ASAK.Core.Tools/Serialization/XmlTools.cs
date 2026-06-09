using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RIVS.ASAK.Core.Tools.Serialization
{
    public static class XmlTools
    {
        private const string EnumerableElementNode = "Item";
        private const string EnumerableElementValueNode = "Val";

        // кэш сералайзеров по типам
        private static Dictionary<Type, XmlSerializer> _cash;
        // объект синхронизации
        private static readonly object Locker = new object();

        /// <summary>
        /// Использовать кэширование сериалайзеров.
        /// </summary>
        public static bool UseCash
        {
            get
            {
                bool res;
                lock (Locker)
                    res = (_cash != null);
                return res;
            }
            set
            {
                if (UseCash == value)
                    return;

                lock (Locker)
                {
                    if (value)
                        _cash = new Dictionary<Type, XmlSerializer>();
                    else
                    {
                        _cash.Clear();
                        _cash = null;
                    }
                }
            }
        }


        public static Guid ReadParamFromXmlAsGuid(XElement parent, String paramName)
        {
            if (null == parent)
                return Guid.Empty;
            var param = parent.Element(paramName);
            return null == param 
                ? Guid.Empty 
                : param.Value.ParseGuid(Guid.Empty);
        }

        public static Guid ParseGuid(this string source, Guid defValue)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return defValue;
            }
            Guid ret;
            return Guid.TryParse(source, out ret)
                ? ret : defValue;
        }

        public static object ReadPropertyValueFromXml(XElement xelement, String propertyName, Type type)
        {
            var xproperty = xelement.Elements(propertyName);
            if (xproperty.Count() != 1)
                throw new ApplicationException("Что-то не так с данными для конфигурации! Должен быть только один элемент с уникальным именем!");
            var xvalues = xproperty.Elements();
            if (xvalues.Count() != 1)
                throw new ApplicationException("Что-то не так с данными для конфигурации! Должен быть только одно значение!");
            var xvalue = xvalues.First();
            // Десериализуем его
            var serializer = GetSerializer(type, true);
            using (var reader = xvalue.CreateReader())            
                return serializer.Deserialize(reader);
        }

        public static object ReadParamsFromXml(XElement xelement, Type type)
        {
            if( null == xelement )
            {
                throw new ArgumentNullException( "xelement" );
            }

            var serializer = GetSerializer(type, true);

            // Создается более мощный читатель xml, в частности работающий с бинарными данными(запакованы в Base64)            
            using (var reader = XmlReader.Create(xelement.CreateReader(), null))            
                return serializer.Deserialize(reader);
        }

        /// <summary>
        /// Десереализует XElement в объект указанного типа
        /// </summary>
        public static object Deserialize( this XElement xElement, Type type )
        {
            var serializer = GetSerializer( type, false );
            
            using( var reader = xElement.CreateReader() )
            {
                return serializer.Deserialize( reader );
            }
        }

        /// <summary>
        /// Десереализует XElement в объект указанного типа
        /// </summary>
        public static T Deserialize<T>( this XElement xElement )
        {
            return (T)xElement.Deserialize( typeof( T ) );
        }


        //
        /// <summary>
        /// Сериализуем объект в xml-строку
        /// </summary>
        /// <param name="element">Сериализуемый объект</param>
        /// <param name="useRootXml">Использовать root в готовом XML.</param>
        /// <returns>Возвращает булевый результат проверки.</returns>
        public static string SerializeObjectToXML(object element, bool useRootXml = true)
        {
            // Создается более мощный читатель xml, в частности работающий с бинарными данными(запакованы в Base64)
            var serializer = GetSerializer(element.GetType(), useRootXml);

            var strBuilder = new StringBuilder();
            using (var writer = XmlWriter.Create(strBuilder))
            {
                serializer.Serialize(writer, element);
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// Сереализует объект в XElement указанного типа
        /// </summary>
        public static string Serialize( this object element )
        {
            return SerializeObjectToXML( element );
        }

        /// <summary>
        /// Метод проверки наличия атрибута в XML описании.
        /// </summary>
        /// <param name="cfg">XML описание сущности.</param>
        /// <param name="attributeName">Имя атрибута.</param>
        /// <returns>Возвращает булевый результат проверки.</returns>
        public static bool ContainAttribute(XElement cfg, string attributeName)
        {
            if (cfg != null && !string.IsNullOrEmpty(attributeName))
                return cfg.Attribute(attributeName) != null;
            return false;
        }

        /// <summary>
        /// Метод проверки наличия дочернего элемента в XML описании.
        /// </summary>
        /// <param name="cfg">XML описание сущности.</param>
        /// <param name="elementName">Имя элемента.</param>
        /// <returns>Возвращает булевый результат проверки.</returns>
        public static bool ContainElement(XElement cfg, string elementName)
        {
            if (cfg != null && !string.IsNullOrEmpty(elementName))
                return cfg.Element(elementName) != null;
            return false;
        }

        /// <summary>
        /// Reads attribute value.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Attribute name.</param>
        /// <returns>Attribute value if succeeded, empty string otherwise.</returns>
        public static string ReadAttributeValue(XElement parent, string name)
        {
            if (null == parent)
                return string.Empty;
            var attribute = parent.Attribute(name);
            return null == attribute ? string.Empty : attribute.Value;
        }

        /// <summary>
        /// Reads attribute value.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Attribute name.</param>
        /// <returns>Guid value if succeeded, Guid.Empty otherwise.</returns>
        public static Guid ReadAttributeValueAsGuid(XElement parent, string name)
        {
            if (null == parent)
                return Guid.Empty;
            var attribute = parent.Attribute(name);
            if(null == attribute || string.IsNullOrWhiteSpace(attribute.Value))
                return Guid.Empty;
            Guid ret;
            Guid.TryParse(attribute.Value, out ret);
            return ret;
        }

        /// <summary>
        /// Reads attribute value.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Attribute name.</param>
        /// <returns>Integer value if succeeded, 0 otherwise.</returns>
        public static int ReadAttributeValueAsInt(XElement parent, string name)
        {
            return ReadAttributeValueAsInt(parent, name, 0);
        }

        /// <summary>
        /// Чтение атрибута типа enum. Предполагается что он записан как .ToString()
        /// </summary>
        /// <typeparam name="T">Тип enum</typeparam>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Attribute name.</param>
        /// <param name="defaultValue">value for parse error</param>
        /// <returns>Enum value if succeeded, defaultValue otherwise.</returns>
        public static T ReadAttributeValueAsEnum<T>(XElement parent, string name, T defaultValue) where T : struct 
        {
            if (null == parent)
                return defaultValue;
            var attribute = parent.Attribute(name);
            if (null == attribute || string.IsNullOrWhiteSpace(attribute.Value))
                return defaultValue;
            T ret;
            return Enum.TryParse(attribute.Value, out ret)
                ? ret
                : defaultValue;
        }

        /// <summary>
        /// Reads attribute value.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Attribute name.</param>
        /// <param name="defaultValue">value for parse error</param>
        /// <returns>Integer value if succeeded, defaultValue otherwise.</returns>
        public static int ReadAttributeValueAsInt(XElement parent, string name, int defaultValue)
        {
            if (null == parent)
                return defaultValue;
            var attribute = parent.Attribute(name);
            if (null == attribute || string.IsNullOrWhiteSpace(attribute.Value))
                return defaultValue;
            int ret;
            return int.TryParse(attribute.Value, out ret) 
                ? ret 
                : defaultValue;
        }

        public static XAttribute GetDateTimeAsAttribute(string name, DateTime value)
        {
            var ticks = value.Ticks;
            return new XAttribute(name, ticks);
        }

        public static DateTime ReadAttributeValueAsDateTime(XElement parent, string name, DateTime defaultValue)
        {
            var val = ReadAttributeValueAsLong(parent, name, -1);
            if (val == -1) return defaultValue;
            if ((val < DateTime.MinValue.Ticks) || (val > DateTime.MaxValue.Ticks)) return defaultValue;
            return new DateTime(val);
        }

        /// <summary>
        /// Reads attribute value.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Attribute name.</param>
        /// <returns>Long value if succeeded, 0 otherwise.</returns>
        public static long ReadAttributeValueAsLong(XElement parent, string name)
        {
            return ReadAttributeValueAsLong(parent, name, 0);
        }

        /// <summary>
        /// Reads attribute value.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Attribute name.</param>
        /// <param name="defaultValue">value for parse error</param>
        /// <returns>Long value if succeeded, defaultValue otherwise.</returns>
        public static long ReadAttributeValueAsLong(XElement parent, string name, long defaultValue)
        {
            if (null == parent)
                return defaultValue;
            var attribute = parent.Attribute(name);
            if (null == attribute || string.IsNullOrWhiteSpace(attribute.Value))
                return defaultValue;
            long ret;
            return long.TryParse(attribute.Value, out ret)
                ? ret
                : defaultValue;
        }

        /// <summary>
        /// Reads attribute value.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Attribute name.</param>
        /// <returns>UInteger value if succeeded, 0 otherwise.</returns>
        public static uint ReadAttributeValueAsUInt(XElement parent, string name)
        {
            if (null == parent)
                return 0;
            var attribute = parent.Attribute(name);
            if (null == attribute || string.IsNullOrWhiteSpace(attribute.Value))
                return 0;

            uint ret;
            uint.TryParse(attribute.Value, out ret);
            return ret;
        }

        /// <summary>
        /// Reads attribute value.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Attribute name.</param>
        /// <returns>Integer value if succeeded, 0 otherwise.</returns>
        public static T ReadAttributeValueAs<T>(XElement parent, string name)
        {
            if (null == parent)
                return default(T);
            var attribute = parent.Attribute(name);
            if (null == attribute || string.IsNullOrWhiteSpace(attribute.Value))
                return default(T);
            try
            {
                var ret = (T)Convert.ChangeType(attribute.Value, typeof(T), CultureInfo.InvariantCulture);
                return ret;
            }
            catch (Exception e)
            {
                Debug.Assert(false, e.ToString());
            }

            return default(T);
        }

        public static XAttribute AddAttribute(string name, object value)
        {
            return new XAttribute(name, Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        public static XElement GetIEnumerableAsElement<T>(string name, IEnumerable<T> value, Func<T, string> elementSerializeFunction)
        {
            var element = new XElement(name);
            foreach (var item in value)
            {
                element.Add(new XElement(EnumerableElementNode,
                    new XAttribute(EnumerableElementValueNode, elementSerializeFunction(item))));
            }
            return element;
        }

        public static IEnumerable<T> ReadElementAsIEnumerable<T>(XElement element, Func<string, T> deserializeFunction)
        {
            if (element != null)
            {
                foreach (var node in element.Elements(EnumerableElementNode))
                {
                    var attr = node.Attribute(EnumerableElementValueNode);
                    if (attr != null)
                    {
                        yield return deserializeFunction(attr.Value);
                    }
                }
            }
        }

        public static XElement GetIEnumerableAsElement<T>(string name, IEnumerable<T> value, Func<T, XElement> elementSerializeFunction)
        {
            var element = new XElement(name);
            foreach (var item in value)
            {
                element.Add(elementSerializeFunction(item));
            }
            return element;
        }

        public static IEnumerable<T> ReadElementAsIEnumerable<T>(XElement element, Func<XElement, T> deserializeFunction)
        {
            if (element != null)
            {
                foreach (var node in element.Elements(EnumerableElementNode))
                {
                    yield return deserializeFunction(node);
                }
            }
        }

        /// <summary>
        /// Метод получения сериалайзера по типу.
        /// </summary>
        /// <param name="type">Заданный тип.</param>
        /// <param name="useRootAttribute">При создании сериализатора используется атрибут XmlRootAttribute</param>
        /// <returns>Возвращает сериалайзер типа.</returns>
        public static XmlSerializer GetSerializer( Type type, bool useRootAttribute )
        {
            XmlSerializer result;
            lock (Locker)
            {
                if (_cash != null && _cash.ContainsKey(type))
                    result = _cash[type];
                else
                {
                    result = ( useRootAttribute ) ? new XmlSerializer(type, new XmlRootAttribute()) : new XmlSerializer( type );
                    if (_cash != null)
                        _cash[type] = result;
                }
            }
            return result;
        }

        /// <summary>
        /// Reads attribute value.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Attribute name.</param>
        /// <returns>Bool value if succeeded, false otherwise.</returns>
        public static bool ReadAttributeValueAsBool(XElement parent, string name)
        {
            if (null == parent)
                return false;
            var attribute = parent.Attribute(name);
            if (null == attribute || string.IsNullOrWhiteSpace(attribute.Value))
                return false;
            bool ret;
            bool.TryParse(attribute.Value, out ret);
            return ret;
        }
    }
}
