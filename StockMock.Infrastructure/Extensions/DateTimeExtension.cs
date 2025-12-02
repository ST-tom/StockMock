namespace StockMock.Infrastructure.Extensions
{
    public static class DateTimeExtension
    {
        public static DateOnly ToDateOnly( this DateTime dateTime)
        {
            return DateOnly.FromDateTime(dateTime);
        }
    }
}
