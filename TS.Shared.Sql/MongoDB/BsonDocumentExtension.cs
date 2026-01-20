using MongoDB.Bson;
using System.Globalization;
using TS.Shared.Sql.MongoDB;

namespace TS.Shared.Sql.MongoDB
{
    public static class BsonDocumentExtension
    {
        public static T TryGetValue<T>(this BsonDocument document, string key, T defaultValue = default!)
        {
            if (document.Contains(key))
                return document[key].ToType<T>();
            else
                return defaultValue;
        }

        public static T TryGetValueIfNotThrow<T>(this BsonDocument document, string key, T defaultValue = default!)
        {
            if (document.Contains(key))
                return document[key].ToType(defaultValue);
            else
                throw new KeyNotFoundException($"MongoDB文档不存在键值{key}");
        }

        public static T ToType<T>(this BsonValue bsonValue, T defaultValue = default!)
        {
            if (bsonValue == null || bsonValue.IsBsonNull)
            {
                if (typeof(T) == typeof(BsonDocument))
                    return (T)(object)new BsonDocument();

                return defaultValue;
            }

            if (typeof(T) == typeof(BsonDocument))
            {
                return (T)(object)(bsonValue.IsBsonDocument ? bsonValue.AsBsonDocument : []);
            }

            try
            {
                // 优先使用MongoDB内置转换（适配Bson原生基础类型）
                return (T)Convert.ChangeType(bsonValue, typeof(T), CultureInfo.InvariantCulture);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static object ToObject(this BsonValue value)
        {
            var type = value.BsonType;
            return type switch
            {
                BsonType.String => value.AsString,
                BsonType.Int32 => value.AsInt32,
                BsonType.Int64 => value.AsInt64,
                BsonType.Double => value.AsDouble,
                BsonType.Boolean => value.AsBoolean,
                BsonType.DateTime => value.AsUniversalTime,
                BsonType.ObjectId => value.AsObjectId,
                BsonType.Decimal128 => (object)value.AsDecimal128,
                _ => value.AsBsonValue,
            };
        }
    }
}
