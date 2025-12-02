namespace StockMock.Domain.Common
{
    public class BusinessExcption : Exception
    {
        public BusinessExcption() : base()
        { 
        }

        public BusinessExcption(string message) : base(message)
        { 
        }

        public BusinessExcption(string message, Exception innerException) : base(message, innerException)
        {
        }

        public static BusinessExcption Null(string text)
        {
            throw new BusinessExcption($"{text}不能为空");
        }
    }
}
