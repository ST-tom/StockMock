using Newtonsoft.Json;

namespace StockMock.Domain.Common
{
    public class ApiResult
    {
        const string noLogin = "当前用户尚未登陆，请登陆后重新尝试";

        const string noAuthory = "当前用户无此操作权限，请联系管理人员授权后重新尝试";

        public int code;

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

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class ResultCode
    {
        public const int Success = 1;

        public const int Failure = 10;

        public const int NoLogin = 11;

        public const int NoAuthory = 12;

        /// <summary>
        /// 未预支的错误（未特殊处理的错误）
        /// </summary>
        public const int OutFailure = 19;
    }
}
