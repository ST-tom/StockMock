using System.Text.Json;
using System.Text.Json.Serialization;
using TS.Shared.Extension;

namespace TS.Shared.Json
{
    public static class JsonExtension
    {
        /// <summary>
        /// 对象转JSON字符串
        /// </summary>
        /// <param name="obj">待序列化的对象</param>
        /// <returns>JSON字符串</returns>
        public static string? ToJson(this object? obj, JsonSerializerOptions? options = default) => 
            obj == null ? null : JsonSerializer.Serialize(obj, options ?? JsonGlobalConfig.DefaultOptions);

        /// <summary>
        /// JSON字符串转指定类型对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="json">JSON字符串</param>
        /// <param name="options">自定义反序列化配置</param>
        /// <returns>反序列化后的对象</returns>
        public static T? ToObject<T>(this string? json, JsonSerializerOptions? options = default)
        {
            if (options == null) 
                throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(json))
            {
                if (typeof(T).IsValueType && Nullable.GetUnderlyingType(typeof(T)) == null)
                    throw new ArgumentNullException(nameof(json), "JSON字符串不能为空（目标类型为非可空值类型）");

                return default;
            }

            return JsonSerializer.Deserialize<T>(json, options ?? JsonGlobalConfig.DefaultOptions);
        }

        /// <summary>
        /// JSON字符串转指定类型对象
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <param name="type">目标类型</param>
        /// <returns>反序列化后的对象</returns>
        public static object? ToObject(this string? json, Type type, JsonSerializerOptions? options = default)
        {
            if (type == null) 
                throw new ArgumentNullException(nameof(type));

            if (string.IsNullOrWhiteSpace(json)) 
                return null;

            return JsonSerializer.Deserialize(json, type, options ?? JsonGlobalConfig.DefaultOptions);
        }

        public static void AddDefaultJsonOptions(this JsonSerializerOptions options)
        {
            options.PropertyNamingPolicy = JsonGlobalConfig.DefaultOptions.PropertyNamingPolicy;
            options.DefaultIgnoreCondition = JsonGlobalConfig.DefaultOptions.DefaultIgnoreCondition;         
            options.ReferenceHandler = JsonGlobalConfig.DefaultOptions.ReferenceHandler;
            options.WriteIndented = JsonGlobalConfig.DefaultOptions.WriteIndented;
            options.Encoder = JsonGlobalConfig.DefaultOptions.Encoder;
            options.AllowTrailingCommas = JsonGlobalConfig.DefaultOptions.AllowTrailingCommas ;
            options.ReadCommentHandling = JsonGlobalConfig.DefaultOptions.ReadCommentHandling;
            options.Converters.Clear();
            JsonGlobalConfig.DefaultOptions.Converters.ForEach(c => options.Converters.Add(c));
        }
    }
}
