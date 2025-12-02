using System.ComponentModel;
using System.Reflection;

namespace StockMock.Infrastructure.Extensions
{
    public static class EnumExtension
    {
        /// <summary>
        /// 扩展方法，获得枚举的Description
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <param name="nameInstend">当枚举没有定义DescriptionAttribute,是否用枚举名代替，默认使用</param>
        /// <returns>枚举的Description</returns>
        public static string? GetDescription(this Enum value, bool nameInstend = true)
        {
            Type type = value.GetType();
            string? name = Enum.GetName(type, value);

            if (name == null)
                return null;

            FieldInfo? field = type.GetField(name);
            if (field == null)
                return null;

            DescriptionAttribute? attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (attribute == null && nameInstend == true)
            {
                return name;
            }
            return attribute?.Description;
        }

        public static Dictionary<int, string> ToDictionary(this Type t)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            Type enumType = t.GetType();
            var fieldstrs = Enum.GetNames(enumType);
            foreach (var fieldstr in fieldstrs)
            {
                var field = enumType.GetField(fieldstr);
                string description = string.Empty;

                if (field == null)
                    continue;

                DescriptionAttribute? des = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

                if (des != null)
                    description = des.Description;   //属性描述
                else
                    description = fieldstr;  //描述不存在取字段名称

                dic.Add((int)Enum.Parse(enumType, fieldstr), description);
            }
            return dic;
        }
    }
}
