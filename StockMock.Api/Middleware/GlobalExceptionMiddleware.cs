using System.Net;
using TS.Shared.Excption;
using TS.Shared.WebApi;

namespace StockMock.Api.Middleware
{
    public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                HttpStatusCode[] waitHandleCodes = [HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden, HttpStatusCode.NotFound];

                var statusCode = (HttpStatusCode)context.Response.StatusCode;
                if (waitHandleCodes.Contains(statusCode))
                {
                    var request = context.Request;
                    var path = request.Path.ToString().ToLower();
                    if (path.StartsWith("/api") || request.Headers.Accept.ToString().Contains("application/json"))
                    {
                        ApiResult result = ApiResult.OK();
                        switch (statusCode)
                        {
                            case HttpStatusCode.Unauthorized:
                                await ApiResult.NoLogin().ExecuteResultAsync(context);
                                break;
                            case HttpStatusCode.Forbidden:
                                await ApiResult.NoAuthory().ExecuteResultAsync(context);
                                break;
                            case HttpStatusCode.NotFound:
                                await ApiResult.NotFound().ExecuteResultAsync(context);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (BizException ex)
            {
                await ApiResult.Err(ex).ExecuteResultAsync(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetFullMessageAndTrace());
                await ApiResult.Err(ex).ExecuteResultAsync(context);
            }
        }
    }

    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalException(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
