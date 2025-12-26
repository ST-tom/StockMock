namespace TS.Shared.Excption
{
    public class ApplicationExcption : Exception
    {
        public ApplicationExcption() : base()
        { 
        }

        public ApplicationExcption(string message) : base(message)
        { 
        }

        public ApplicationExcption(string message, Exception innerException) : base(message, innerException)
        {
        }

        public static ApplicationExcption Null(string text)
        {
            throw new ApplicationExcption($"{text}不能为空");
        }
    }
}
