namespace StockMock.Infrastructure.Utils
{
    public class DateTimeUtil
    {
        public static DateOnly GetCurrentYear()
        {
            return DateOnly.FromDateTime(DateTime.Now);
        }

        public static DateOnly GetLastYear()
        {
            return DateOnly.FromDateTime(DateTime.Now.AddYears(-1));
        }

        public static DateOnly GetLastMonth()
        {
            return DateOnly.FromDateTime(DateTime.Now.AddMonths(-1));
        }

        public static DateOnly GetCurrentMonth()
        {
            return DateOnly.FromDateTime(DateTime.Now.AddDays(-DateTime.Now.Day + 1));
        }

        public static DateOnly GetNextMonth()
        {
            return DateOnly.FromDateTime(DateTime.Now.AddMonths(1));
        }

        public static DateOnly GetNextYear()
        {
            return DateOnly.FromDateTime(DateTime.Now.AddYears(1));
        }

        public static DateOnly GetCurrentDay()
        {
            return DateOnly.FromDateTime(DateTime.Now);
        } 
    }
}
