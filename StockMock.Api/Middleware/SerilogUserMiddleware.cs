using Serilog.Context;
using TS.Shared.User;

namespace StockMock.Api.Middleware
{
    public class SerilogUserMiddleware
    {
        private readonly RequestDelegate _next;

        public SerilogUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUser user)
        {
            // 从IUser中获取用户信息
            if (user.Id > 0)
            {
                LogContext.PushProperty("UserId", user.Id);
            }

            if (!string.IsNullOrEmpty(user.Name))
            {
                LogContext.PushProperty("UserName", user.Name);
            }

            // 处理请求
            await _next(context);
        }
    }

    public static class SerilogUserMiddlewareExtensions
    {
        public static IApplicationBuilder UseSerilogUser(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SerilogUserMiddleware>();
        }
    }
}