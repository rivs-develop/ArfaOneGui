using System.IO;
using System.Runtime.Serialization;

namespace RIVS.ASAK.Core.Tools.Serialization
{
    public interface ICustomSerializable
    {
        void CustomSerialize(Stream stream, StreamingContext streamingContext);
    }

    public interface ICustomSerializationFactory
    {
        void Serialize(object data, Stream stream, StreamingContext streamingContext);
        object Deserialize(Stream stream, StreamingContext streamingContext);
        bool IsCustomSerializable(object data);
    }


}
