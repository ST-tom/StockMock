namespace TS.Shared.Excption
{
    public class BizException : Exception
    {
        public BizException() : base()
        {
        }

        public BizException(string message) : base(message)
        {
        }

        public BizException(string message, Exception innerException) : base(message, innerException)
        {

        }

        public static BizException Null(string text)
        {
            return new BizException($"{text}不能为空");
        }
    }
}
