using System.Text;

namespace TS.Shared.Excption
{
    public static class ExceptionExtension
    {
        /// <summary>
        /// 获取异常的完整信息
        /// </summary>
        public static string GetFullMessageAndTrace(this Exception ex, int maxLength = 2048)
        {
            if (ex == null)
                return string.Empty;

            StringBuilder builder = new();

            int count = 0;
            Exception? currentEx = ex;
            while (currentEx != null)
            {
                count += currentEx.Message.Length + currentEx.StackTrace?.Length ?? 0;
                if (count > maxLength)
                    return builder.ToString();

                builder.AppendLine($"{currentEx.Message} : {currentEx.StackTrace}");
                currentEx = currentEx.InnerException;
            }

            return builder.ToString();
        }
    }
}
