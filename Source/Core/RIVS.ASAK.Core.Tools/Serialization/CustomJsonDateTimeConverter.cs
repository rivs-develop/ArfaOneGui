using Newtonsoft.Json;
using System;

namespace RIVS.ASAK.Core.Tools.Serialization
{
    public class CustomJsonDateTimeConverter : JsonConverter
    {
        public const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
        public const string DateTimeOffsetFormat = "yyyy-MM-ddTHH:mm:sszzz";

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var result = "";
            if (value == null)
            {
                writer.WriteNull();
            }

            if (value.GetType() == typeof(DateTimeOffset) || value.GetType() == typeof(DateTimeOffset?))
            {
                result = ((DateTimeOffset)value).ToString(DateTimeOffsetFormat);
            }
            else
            {
                result = ((DateTime)value).ToString(DateTimeFormat);
            }

            writer.WriteValue(result);
        }

        public override object ReadJson(JsonReader reader, Type type, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value;
            if (value == null)
            {
                return null;
            }

            var valueType = value.GetType();

            if (valueType == typeof(DateTimeOffset))
            {
                var datetimeoffset = (DateTimeOffset)reader.Value;

                if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
                {
                    return datetimeoffset;
                }

                return datetimeoffset.DateTime;
            }

            if (typeof(DateTime).IsAssignableFrom(valueType))
            {
                return (DateTime)value;
            }

            return null;
        }


        public override bool CanConvert(Type type)
        {
            return typeof(DateTime?).IsAssignableFrom(type) || typeof(DateTimeOffset?).IsAssignableFrom(type) || type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?);
        }
    }
}
