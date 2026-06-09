using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RIVS.ASAK.Core.Tools.Serialization
{
    /// <summary>
    /// Сериализует/десериализует объект в/из строки
    /// </summary>
    /// <typeparam name="TSerializedType">Тип сериализуемого объекта</typeparam>
    public static class TypeToStringSerializeHelper<TSerializedType> where TSerializedType : new()
    {
        public static string Serialize(TSerializedType obj)
        {
            var serializer = new XmlSerializer(typeof(TSerializedType));
            var sb = new StringBuilder();
            var writerSettings = new XmlWriterSettings
            {
                Indent = false,
                NewLineHandling = NewLineHandling.None,
                OmitXmlDeclaration = true
            };
            var xmlWriterEnt = XmlWriter.Create(sb, writerSettings);
            var emptyNameSpace = new XmlSerializerNamespaces();
            emptyNameSpace.Add("", "");
            serializer.Serialize(xmlWriterEnt, obj, emptyNameSpace);
            return sb.ToString();
        }

        public static TSerializedType Deserialize(string serialized)
        {
            if (String.IsNullOrEmpty(serialized))
                return new TSerializedType();

            var serializer = new XmlSerializer(typeof(TSerializedType));
            var stringReader = new StringReader(serialized);
            var readSettings = new XmlReaderSettings();
            var xmlReader = XmlReader.Create(stringReader, readSettings);
            return (TSerializedType)serializer.Deserialize(xmlReader);
        }
    }
}
