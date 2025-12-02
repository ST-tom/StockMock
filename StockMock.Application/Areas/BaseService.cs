using AutoMapper;
using StockMock.Domain.Common.Caches;
using StockMock.Infrastructure.Database;

namespace StockMock.Application.Areas
{
    public class BaseService 
    {
        protected ApplicationDbContext _context;

        protected IMapper _mapper;

        protected CancellationToken _cancellationToken;

        protected LocalCacheManager _loaclCache;

        public BaseService(ApplicationDbContext context, IMapper mapper, CancellationToken cancellationToken, LocalCacheManager loaclCache)
        {
            _context = context;
            _mapper = mapper;
            _cancellationToken = cancellationToken;
            _loaclCache = loaclCache;
        }
    }
}
