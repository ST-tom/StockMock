using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockMock.Service.Areas.Configs;
using StockMock.Service.Areas.Mocks;
using StockMock.Service.Areas.Stocks;
using TS.Shared.Util;

namespace StockMock.Service
{
    public static class DependencyInjection
    {
        public static void AddServiceDependency(this IHostApplicationBuilder builder)
        {

            builder.Services.AddSingleton<ExcelUtil>();
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<ConfigProfile>();
                cfg.AddProfile<MockProfile>();
                cfg.AddProfile<StockProfile>();    
            });
        }
    }
}
