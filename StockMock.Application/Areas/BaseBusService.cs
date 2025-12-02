using AutoMapper;
using StockMock.Application.Areas.Configs.Services;
using StockMock.Domain.Common.Caches;
using StockMock.Infrastructure.Database;

namespace StockMock.Application.Areas
{
    public class BaseBusService : BaseService
    {
        protected DayService _dayService;

        public BaseBusService(ApplicationDbContext context, IMapper mapper, CancellationToken cancellationToken, LocalCacheManager localCache, DayService dayService) : base(context, mapper, cancellationToken, localCache)
        {
            _dayService = dayService;
        }

    }
}
