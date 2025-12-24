using AutoMapper;
using Microsoft.Extensions.Logging;
using StockMock.Domain.Common.Caches;
using StockMock.Infrastructure.Database;

namespace StockMock.Application.Areas
{
    public class BaseService<T>(ApplicationDbContext context, IMapper mapper, CancellationToken cancellationToken, LocalCacheManager loaclCache, ILogger<T> logger)
        where T : class
    {
        protected ApplicationDbContext _context = context;

        protected IMapper _mapper = mapper;

        protected CancellationToken _cancellationToken = cancellationToken;

        protected LocalCacheManager _loaclCache = loaclCache;

        protected ILogger<T> _logger = logger;
    }
}
