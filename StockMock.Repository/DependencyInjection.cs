using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

namespace StockMock.Repository
{
    public static class DependencyInjection
    {
        public static void AddRepositoryDependency(this IHostApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContext");

            builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

            builder.Services.AddDbContext<ApplicationDbContext>((sp, options) => {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseSqlServer(connectionString);
            });
        }
    }
}
