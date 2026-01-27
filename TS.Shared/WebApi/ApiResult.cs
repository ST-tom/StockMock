using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using System.Text.Json;
using TS.Shared.Excption;
using TS.Shared.Extension;
using TS.Shared.Json;

namespace TS.Shared.WebApi
{
    public class ApiResult : IActionResult
    {
        const string noLogin = "当前用户尚未登陆，请登陆后重新尝试";

        const string noAuthory = "当前用户无此操作权限，请联系管理人员授权后重新尝试";

        public string code;

        public bool isOk;

        public string? message;

        public object? data;

        public string? trace;

        public string? traceId;

        public static ApiResult OK(string? traceId = default, string? trace = default)
        {
            return new ApiResult()
            {
                isOk = true,
                code = ResultCode.Success,
                trace = trace,
                traceId = traceId,
            };
        }

        public static ApiResult OK(string? message, object data, string? traceId = default, string? trace = default)
        {
            return new ApiResult()
            {
                isOk = true,
                code = ResultCode.Success,
                message = message,
                data = data,
                trace = trace,
                traceId = traceId,
            };
        }

        public static ApiResult OK(object data, string? traceId = default, string? trace = default)
        {
            return new ApiResult()
            {
                isOk = true,
                code = ResultCode.Success,
                data = data,
                trace = trace,
                traceId = traceId,
            };
        }

        public static ApiResult Err(string message, string? traceId = null, string? trace = null)
        {
            return new ApiResult()
            {
                isOk = false,
                code = ResultCode.Failure,
                message = message,
                trace = trace,
                traceId = traceId,
            };
        }

        public static ApiResult Err(Exception ex, string? traceId = null, string? trace = null)
        {
            return new ApiResult()
            {
                isOk = false,
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
                isOk = false,
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
                isOk = false,
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
                trace = trace ?? ex.GetFullMessageAndTrace(),
                traceId = traceId,
            };
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var response = context.HttpContext.Response;
            response.StatusCode = HttpStatusCode.OK.ToInt();
            response.ContentType = "application/json; charset=utf-8";

            // 序列化响应对象
            var jsonOptions = JsonGlobalConfig.DefaultOptions;
            var json = JsonSerializer.Serialize(this, jsonOptions);

            await response.WriteAsync(json, Encoding.UTF8);
        }     
    }

    public class ResultCode
    {
        public const string Success = "00001";

        public const string Failure = "00100";

        public const string NoLogin = "00201";

        public const string NoAuthory = "00202";

        /// <summary>
        /// 未预知的错误（未处理的错误）
        /// </summary>
        public const string OutFailure = "00000";
    }
}
