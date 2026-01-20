using System.Text.Json;
using System.Text.Json.Serialization;

namespace TS.Shared.Json
{
    public class JsonConverterObject : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            // 关键：判断目标类型是否为object（或可赋值给object），返回true以启用转换器
            // 这里直接返回true for object类型，确保所有object类型的处理都命中该转换器
            return typeToConvert == typeof(object);
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // 根据JSON令牌类型，转换为对应的原生CLR类型
            return reader.TokenType switch
            {
                JsonTokenType.Null => null!,
                JsonTokenType.String => reader.GetString()!, // 装箱为string
                JsonTokenType.Number => ReadNumberValue(ref reader), // 装箱为int/long/double
                JsonTokenType.True => true, // 装箱为bool
                JsonTokenType.False => false, // 装箱为bool
                JsonTokenType.StartObject => ReadObject(ref reader, options), // 转为Dictionary<string, object>
                JsonTokenType.StartArray => ReadArray(ref reader, options), // 转为List<object>
                _ => throw new JsonException($"不支持的JSON令牌类型: {reader.TokenType}")
            };
        }

        // 序列化：将原生CLR类型拆箱后写入JSON
        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            // 根据CLR类型拆箱并写入对应JSON值
            Type valueType = value.GetType();
            if (valueType == typeof(string))
            {
                writer.WriteStringValue((string)value);
            }
            else if (valueType == typeof(int))
            {
                writer.WriteNumberValue((int)value);
            }
            else if (valueType == typeof(long))
            {
                writer.WriteNumberValue((long)value);
            }
            else if (valueType == typeof(double))
            {
                writer.WriteNumberValue((double)value);
            }
            else if (valueType == typeof(bool))
            {
                writer.WriteBooleanValue((bool)value);
            }
            else if (value is Dictionary<string, object> dict)
            {
                writer.WriteStartObject();
                foreach (var kvp in dict)
                {
                    writer.WritePropertyName(kvp.Key);
                    Write(writer, kvp.Value, options);
                }
                writer.WriteEndObject();
            }
            else if (value is List<object> list)
            {
                // 处理列表
                writer.WriteStartArray();
                foreach (var item in list)
                {
                    Write(writer, item, options);
                }
                writer.WriteEndArray();
            }
            else
            {
                JsonSerializer.Serialize(writer, value, valueType, options);
            }
        }

        private static object ReadNumberValue(ref Utf8JsonReader reader)
        {
            if (reader.TryGetInt32(out int intVal)) return intVal;
            if (reader.TryGetInt64(out long longVal)) return longVal;
            if (reader.TryGetDecimal(out decimal decVal)) return decVal;
            return reader.GetDouble(); // 兜底为double
        }

        private Dictionary<string, object> ReadObject(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var dict = new Dictionary<string, object>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject) return dict;
                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("期望JSON属性名");

                string key = reader.GetString()!;
                reader.Read(); // 移动到属性值
                object value = Read(ref reader, typeof(object), options); // 递归解析值
                dict[key] = value;
            }
            return dict;
        }

        private List<object> ReadArray(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var list = new List<object>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray) return list;
                object item = Read(ref reader, typeof(object), options); // 递归解析数组项
                list.Add(item);
            }
            return list;
        }
    }
}
