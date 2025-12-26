using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StockMock.Api.User;
using StockMock.Core.Accounts;
using System.Text;
using TS.Shared.Extension;
using TS.Shared.Json;
using TS.Shared.User;
using TS.Shared.WebApi;

namespace StockMock.Api
{
    public static class DependencyInjection
    {
        public static void AddApiDependency(this IHostApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUser, CurrentUser>();

            builder.Services
                .AddControllers(options =>
                {
                    //全局异常过滤器（暂时不使用，本系统使用 /Error 记录全局异常日志）
                    //options.Filters.Add<GlobalExceptionFilter>();
                }).AddJsonOptions(option => option.JsonSerializerOptions.AddDefaultJsonOptions())
                .ConfigureApiBehaviorOptions(opt =>
                {
                    opt.InvalidModelStateResponseFactory = context =>
                    {
                        //获取验证失败的模型字段 
                        var errors = context.ModelState.Where(e => e.Value?.Errors.Count > 0).Select(e => e.Value?.Errors.First().ErrorMessage).ToList();
                        var str = string.Join("|", errors);
                        var result = ApiResult.Err(str);
                        return new JsonResult(result);
                    };
                });

            builder.Services
                .AddAuthentication(options =>
                {
                    // 设置默认认证方案为JWT
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>
                {
                    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
                    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));

                    // 配置JWT验证参数
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // 核心验证规则
                        ValidateIssuer = true, // 验证发行人
                        ValidIssuer = jwtSettings["Issuer"], // 合法发行人（匹配配置）
                        ValidateAudience = true, // 验证受众
                        ValidAudience = jwtSettings["Audience"], // 合法受众（匹配配置）
                        ValidateIssuerSigningKey = true, // 验证签名密钥
                        IssuerSigningKey = secretKey, // 签名密钥（匹配签发时的密钥）
                    };
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
        }
    }
}

