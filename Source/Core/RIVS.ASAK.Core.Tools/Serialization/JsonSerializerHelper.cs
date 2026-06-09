using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RIVS.ASAK.Core.Tools.Serialization
{
    public static class JsonSerializerHelper
    {
        public static JsonSerializerSettings JsonSettings
        {
            get
            {
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,

                    //DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
                    DateFormatString = CustomJsonDateTimeConverter.DateTimeOffsetFormat,
                    DateParseHandling = DateParseHandling.DateTimeOffset,

                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,

                    NullValueHandling = NullValueHandling.Ignore,

                    Converters = JsonConverters,
                };

                //settings.Converters.Add(new StringEnumConverter() { AllowIntegerValues = true });
                //settings.Converters.Add(new CustomJsonDateTimeConverter());
                //settings.Converters.Add(new CustomJsonCustomValueConverter());

                return settings;
            }
        }

        public static List<JsonConverter> JsonConverters = new List<JsonConverter>();

        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, JsonSettings);
        }

        public static T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data, JsonSettings);
        }

        public static bool TryDeserializeObject<T>(string json, out T result, Action<Exception> catchAction = null)
        {
            try
            {
                result = JsonConvert.DeserializeObject<T>(json, JsonSettings);
                return true;
            }
            catch (Exception exception)
            {
                catchAction?.Invoke(exception);
                result = default;
                return false;
            }
        }

        public static bool TrySerializeObject<T>(T data, out string serialized, Action<Exception> catchAction = null)
        {
            serialized = null;
            try
            {
                serialized = JsonConvert.SerializeObject(data, Formatting.Indented, JsonSettings);
                return true;
            }
            catch (Exception exception)
            {
                catchAction?.Invoke(exception);
                return false;
            }
        }
    }
}
