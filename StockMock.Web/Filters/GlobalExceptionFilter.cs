using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StockMock.Domain.Common;
using StockMock.Infrastructure.Utils;

namespace StockMock.Web.Filters
{
    /// <summary>
    /// 全局异常过滤器
    /// </summary>
    public class GlobalExceptionFilter : IAsyncExceptionFilter
    {
        public Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.ExceptionHandled == false)
            {
                string? controller = context.HttpContext.Request.RouteValues["controller"] as string;
                string? action = context.HttpContext.Request.RouteValues["action"] as string;
                //记录日志
                Log4NetUtil.Error(this, $"控制器{controller}-{action}发生异常" + context.Exception.ToString());

                var result = ApiResult.Err("系统异常，详情请查看日志");

                context.Result = new ContentResult
                {
                    Content = result.ToJson(),
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ContentType = "text/html;charset=utf-8"
                };
            }
            //异常已处理
            context.ExceptionHandled = true;

            return Task.CompletedTask;
        }
    }
}
