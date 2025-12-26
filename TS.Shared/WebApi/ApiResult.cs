namespace TS.Shared.WebApi
{
    public class ApiResult
    {
        const string noLogin = "当前用户尚未登陆，请登陆后重新尝试";

        const string noAuthory = "当前用户无此操作权限，请联系管理人员授权后重新尝试";

        public string code;

        public bool isSuccess;

        public string? message;

        public object? data;

        public string? trace;

        public string? traceId;

        public static ApiResult OK(string? message = null, object? data = null)
        {
            return new ApiResult()
            {
                code = ResultCode.Success,
                message = message,
                data = data,
            };
        }

        public static ApiResult OK(string message, object data, string traceId, string trace)
        {
            return new ApiResult()
            {
                code = ResultCode.Success,
                message = message,
                data = data,
                trace = trace,
                traceId = traceId,
            };
        }

        public static ApiResult OK(string message, string traceId, string? trace = null)
        {
            return new ApiResult()
            {
                code = ResultCode.Success,
                message = message,
                trace = trace,
                traceId = traceId,
            };
        }

        public static ApiResult Err(string message)
        {
            return new ApiResult()
            {
                code = ResultCode.Failure,
                message = message,
            };
        }

        public static ApiResult Err(Exception ex)
        {
            return new ApiResult()
            {
                code = ResultCode.Failure,
                message = ex.Message,
            };
        }

        public static ApiResult Err(Exception ex, string? traceId = null, string? trace = null)
        {
            return new ApiResult()
            {
                code = ResultCode.Failure,
                message = ex.Message,
                trace = trace,
                traceId = traceId,
            };
        }

        public static ApiResult NoLogin(string? traceId = null, string? trace = null)
        {
            return new ApiResult()
            {
                code = ResultCode.NoLogin,
                message = noLogin,
                trace = trace,
                traceId = traceId,
            };
        }

        public static ApiResult NoAuthory(string? traceId = null, string? trace = null)
        {
            return new ApiResult()
            {
                code = ResultCode.NoAuthory,
                message = noAuthory,
                trace = trace,
                traceId = traceId,
            };
        }

        public static ApiResult OutErr(Exception ex, string? traceId = null, string? trace = null)
        {
            return new ApiResult()
            {
                code = ResultCode.OutFailure,
                message = ex.Message,
                trace = ex.StackTrace,
                traceId = traceId,
            };
        }
    }

    public class ResultCode
    {
        public const string Success = "0001";

        public const string Failure = "0010";

        public const string NoLogin = "0011";

        public const string NoAuthory = "0012";

        /// <summary>
        /// 未预知的错误（未处理的错误）
        /// </summary>
        public const string OutFailure = "1000";
    }
}
