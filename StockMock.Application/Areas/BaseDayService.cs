using AutoMapper;
using Microsoft.Extensions.Logging;
using StockMock.Application.Areas.Configs.Services;
using StockMock.Domain.Common.Caches;
using StockMock.Infrastructure.Database;

namespace StockMock.Application.Areas
{
    public class BaseDayService<T>(ApplicationDbContext context, IMapper mapper, CancellationToken cancellationToken, LocalCacheManager localCache, ILogger<T> logger, DayService dayService) : BaseService<T>(context, mapper, cancellationToken, localCache, logger)
        where T : class
    {
        protected DayService _dayService = dayService;
    }
}
