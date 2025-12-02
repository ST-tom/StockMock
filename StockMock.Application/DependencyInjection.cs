using FluentValidation;
using Microsoft.Extensions.Hosting;
using StockMock.Application.Areas;
using StockMock.Infrastructure.Extensions;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddApplicationDependency(this IHostApplicationBuilder builder)
        {
            //AutoMapper自动映射模型
            builder.Services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
            //FluentValidation验证
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            builder.Services.AddScopedByBaseType<BaseService>();
        }
    }
}
