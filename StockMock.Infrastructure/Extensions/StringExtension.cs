namespace StockMock.Infrastructure.Extensions
{
    public static class StringExtension
    {
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

    }
}
