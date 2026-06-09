using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RIVS.ASAK.Core.Tools.Serialization
{
    [Serializable]
    [XmlRoot("Dictionary")]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        private const string XmlTypeTag     = "type";
        private const string XmlKeyValueTag = "KeyValue";
        private const string XmlKeyTag      = "Key";
        private const string XmlValueTag    = "Value";

        public SerializableDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public SerializableDictionary()
        {
        }

        #region IXmlSerializable Members

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void WriteXml(XmlWriter writer)
        {
            Action<string, object> writeAtom = (name, atom) =>
                                                   {
                                                       writer.WriteStartElement(name);
                                                       Type atomType = atom.GetType();
                                                       if (atomType != typeof(TKey))
                                                       {
                                                           writer.WriteAttributeString("",
                                                                                       XmlTypeTag,
                                                                                       "",
                                                                                       atomType.AssemblyQualifiedName);
                                                       }
                                                       new XmlSerializer(atomType).Serialize(writer, atom);
                                                       writer.WriteEndElement();
                                                   };
            foreach (var keyValuePair in this)
            {
                writer.WriteStartElement(XmlKeyValueTag);
                writeAtom(XmlKeyTag, keyValuePair.Key);
                if (keyValuePair.Value != null)
                {
                    writeAtom(XmlValueTag, keyValuePair.Value);
                }
                writer.WriteEndElement();
            }
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }
            var keySerializer = new XmlSerializer(typeof(TKey));
            var valueSerializer = new XmlSerializer(typeof(TValue));
            Func<XmlSerializer, object> readAtom = defaultSerializer =>
                                                       {
                                                           reader.Read();
                                                           var attr = reader.GetAttribute(XmlTypeTag);
                                                           var ser = attr == null
                                                                                   ? defaultSerializer
                                                                                   : new XmlSerializer(Type.GetType(attr));
                                                           var val = ser.Deserialize(reader);
                                                           reader.ReadEndElement();
                                                           return val;
                                                       };
            reader.Read(); // <Dictionary>
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read(); // <KeyValue>
                var key = (TKey)readAtom(keySerializer);
                var val = reader.Name == XmlValueTag ? (TValue)readAtom(valueSerializer) : default(TValue);
                reader.ReadEndElement(); // </KeyValue>
                Add(key, val);
            }
            reader.ReadEndElement(); // </Dictionary>
        }

        #endregion
    }
}
