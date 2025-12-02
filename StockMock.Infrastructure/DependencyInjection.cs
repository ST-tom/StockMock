using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using StockMock.Infrastructure.Database;
using StockMock.Infrastructure.Database.Interceptors;
using StockMock.Infrastructure.Utils;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureDependency(this IHostApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContext");

            builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

            builder.Services.AddDbContext<ApplicationDbContext>((sp, options) => {

                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseSqlite(connectionString);

            });
            //builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            builder.Services.AddSingleton<ExcelUtil>();
        }
    }
}
