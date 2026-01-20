namespace TS.Shared.Extension
{
    public static class StuctExtension
    {
        #region bool

        public static string GetString(this bool value, string trueValue = "是", string falseValue = "否") => value ? trueValue : falseValue;

        #endregion

        #region Type

        /// <summary>
        /// 判断是否为简单类型（string/int/long/double等）
        /// </summary>
        public static bool IsSimpleType(this Type type)
        {
            return type.IsValueType || type == typeof(string);
        }

        #endregion
    }
}
