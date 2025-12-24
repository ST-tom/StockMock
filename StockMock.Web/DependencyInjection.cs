using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using StockMock.Domain.Common;
using StockMock.Domain.Entities.Accounts;
using StockMock.Domain.Users;
using StockMock.Infrastructure.Common;
using StockMock.Infrastructure.Extensions;
using StockMock.Web.UserManager;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddWebDependency(this IHostApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUser, CurrentUser>();

            builder.Services.AddControllersWithViews(options =>
            {
                //全局异常过滤器（暂时不使用，本系统使用 /Error 记录全局异常日志）
                //options.Filters.Add<GlobalExceptionFilter>();
            }).AddJsonOptions(options =>
            {
                //原样序列化
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                //不使用编码
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                //忽略循环引用
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                //DateTime格式化
                options.JsonSerializerOptions.Converters.Add(new JsonConverterDateTime());
            }).ConfigureApiBehaviorOptions(opt =>
            {
                opt.InvalidModelStateResponseFactory = context =>
                {
                    //获取验证失败的模型字段 
                    var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .Select(e => e.Value?.Errors.First().ErrorMessage)
                    .ToList();
                    var str = string.Join("|", errors);
                    //设置返回内容
                    var result = ApiResult.Err(str);
                    return new JsonResult(result);
                };
            }).AddRazorRuntimeCompilation();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
              .AddCookie(options =>
              {
                  //认证路径
                  options.LoginPath = new PathString("/Account/Login");
                  //授权失败路径
                  options.AccessDeniedPath = new PathString("/Home/Denied");
              });

            //基于策略的授权
            builder.Services.AddAuthorization(options =>
            {
                foreach (var role in typeof(AccountRole).ToDictionary())
                {
                    options.AddPolicy(role.Value, policy => policy.RequireRole(role.Value));
                }
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession();

        }
    }

}

