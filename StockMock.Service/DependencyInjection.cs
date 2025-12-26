using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockMock.Repository;
using TS.Shared.Util;

namespace StockMock.Service
{
    public static class DependencyInjection
    {
        public static void AddServiceDependency(this IHostApplicationBuilder builder)
        {

            builder.Services.AddSingleton<ExcelUtil>();
        }
    }
}
