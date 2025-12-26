using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace TS.Shared.Json
{
    public class JsonGlobalConfig
    {
        /// <summary>
        /// 默认 JSON 配置
        /// </summary>
        public static readonly JsonSerializerOptions DefaultOptions = new()
        {
            PropertyNamingPolicy = null,//原样序列化
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,//忽略空值
            Converters = { new JsonStringEnumConverter(), new JsonConverterDateTime() }, //枚举和字符串互相转换，时间转换指定格式
            ReferenceHandler = ReferenceHandler.IgnoreCycles, //忽略循环引用
            WriteIndented = false,//生成字符串不格式
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),//不使用编码  
            AllowTrailingCommas = true, // 兼容JSON末尾逗号
            ReadCommentHandling = JsonCommentHandling.Skip, // 忽略JSON注释
        };
    }
}
