using Microsoft.Extensions.Hosting;
using StockMock.Domain.Common.Caches;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddDomainDependency(this IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton<LocalCacheManager>();
        }
    }
}