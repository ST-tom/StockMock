namespace TS.Shared.Util
{
    public class RetryUtil
    {
        /// <summary>
        /// 运行指定方法，并自动重试
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="maxRetryCount">最大重试次数</param>
        /// <param name="retryIntervalMilliseconds">重试间隔</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<T> RunAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default,int maxRetryCount = 3, int retryIntervalMilliseconds = 1000)
        {
            Exception? resultEx = default;
            int retryCount = 0;
            while(retryCount < maxRetryCount)
            {
                try
                {
                    return await action();
                }
                catch (Exception ex)
                {
                    resultEx = ex;
                    retryCount++;
                    //重试间隔
                    await Task.Delay(TimeSpan.FromMilliseconds(retryIntervalMilliseconds), cancellationToken);
                }
            }

            throw new InvalidOperationException($"[{typeof(T).Name}] 重试{retryCount}次后仍失败，错误信息：{resultEx?.Message}", resultEx);
        }
    }
}
