using System.Globalization;

namespace StockMock.Infrastructure.Extensions
{
    public static class StringExtension
    {
        #region 非空判断

        public static bool IsNullOrEmpty(this string? s)
        {
            if (s != null)
            {
                return s.Length == 0;
            }

            return true;
        }

        public static bool IsNotNullOrEmpty(this string? s)
        {
            if (s != null)
            {
                return !(s.Length == 0);
            }

            return false;
        }

        #endregion

        #region 字符串拼接

        public static string ToJoinString(this IEnumerable<string?> strings, string joinStr = ",")
        {
            if (strings == null || !strings.Any())
                return string.Empty;

            return string.Join(joinStr, strings);
        }

        public static IEnumerable<T> TrySplit<T>(this string s, string separator = ",", IEnumerable<T>? defaultValue = default)
        {
            if (string.IsNullOrEmpty(s))
                return new List<T>();

            return s.Split([separator], StringSplitOptions.RemoveEmptyEntries).Select(x => (T)Convert.ChangeType(x, typeof(T)));
        }

        #endregion

        #region 数据转换

        private static readonly string[] _trueValues = { "true", "1", "yes", "y", "是" };
        private static readonly string[] _falseValues = { "false", "0", "no", "n", "否" };
        private static readonly IFormatProvider _defaultFormat = CultureInfo.CurrentCulture;

        #region 一、整型（按字节/范围从小到大：Byte → Short → Int32 → Int64 → 无符号）
        public static byte ToByte(this string input, byte defaultValue = default)
            => byte.TryParse(input?.Trim(), out byte result) ? result : defaultValue;

        public static short ToShort(this string input, short defaultValue = default)
            => short.TryParse(input?.Trim(), out short result) ? result : defaultValue;

        public static int ToInt32(this string input, int defaultValue = default)
            => int.TryParse(input?.Trim(), out int result) ? result : defaultValue;

        public static long ToInt64(this string input, long defaultValue = default)
            => long.TryParse(input?.Trim(), out long result) ? result : defaultValue;

        public static ushort ToUShort(this string input, ushort defaultValue = default)
            => ushort.TryParse(input?.Trim(), out ushort result) ? result : defaultValue;

        public static ulong ToULong(this string input, ulong defaultValue = default)
            => ulong.TryParse(input?.Trim(), out ulong result) ? result : defaultValue;
        #endregion

        #region 二、布尔型（基础值类型）
        public static bool ToBoolean(this string input, bool defaultValue = default)
        { 
            if (string.IsNullOrWhiteSpace(input))
                return defaultValue;

            var trimInput = input.Trim().ToLowerInvariant();

            if (_trueValues.Contains(trimInput))
                return true;
            if (_falseValues.Contains(trimInput))
                return false;

            return defaultValue;
        }
        #endregion

        #region 小数型
        public static float ToFloat(this string input, float defaultValue = default, IFormatProvider? formatProvider = null)
            => float.TryParse(input?.Trim(), NumberStyles.Float | NumberStyles.AllowThousands, formatProvider ?? _defaultFormat, out float result)
                ? result : defaultValue;

        public static double ToDouble(this string input, double defaultValue = default, IFormatProvider? formatProvider = null)
            => double.TryParse(input?.Trim(), NumberStyles.Float | NumberStyles.AllowThousands, formatProvider ?? _defaultFormat, out double result)
                ? result : defaultValue;

        public static decimal ToDecimal(this string input, decimal defaultValue = default, IFormatProvider? formatProvider = null)
            => decimal.TryParse(input?.Trim(), NumberStyles.Number, formatProvider ?? _defaultFormat, out decimal result)
                ? result : defaultValue;
        #endregion

        #endregion
    }
}
