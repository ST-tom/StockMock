namespace TS.Shared.Extension
{
    public static class DateTimeExtension
    {
        public static DateOnly ToDateOnly( this DateTime dateTime) => DateOnly.FromDateTime(dateTime);

        public static string ToDateTimeString(this DateTime dateTime) => dateTime.ToString("yyyy-MM-dd HH:mm:ss");

        public static string ToDateString(this DateTime dateTime) => dateTime.ToString("yyyy-MM-dd");

        public static string ToTimeString(this DateTime dateTime) => dateTime.ToString("HH:mm:ss");

        public static string ToDateString(this DateOnly dateTime) => dateTime.ToString("yyyy-MM-dd");

        public static string ToTimeString(this TimeOnly dateTime) => dateTime.ToString("HH:mm:ss");

        /// <summary>
        /// 将对象转换为ISO格式的日期时间字符串
        /// </summary>
        /// <param name="obj">要转换的对象，支持DateTime、DateTimeOffset、DateOnly、TimeOnly类型</param>
        /// <returns>返回ISO格式的日期时间字符串</returns>
        public static string ToDateTimeForISOFormat(this object obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            if (obj is DateTime date)
            {
                return date.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            }
            else if (obj is DateTimeOffset dateOffset)
            {
                return dateOffset.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            }
            else if (obj is DateOnly dateOnly)
            {
                return dateOnly.ToString("yyyy-MM-dd");
            }
            else if (obj is TimeOnly timeOnly)
            {
                return timeOnly.ToString("HH:mm:ss.fff");
            }

            throw new ArgumentException($"Object of type '{obj.GetType().Name}' cannot be converted to ISO date time format string");
        }
    }
}
