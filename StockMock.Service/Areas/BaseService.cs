using AutoMapper;
using Microsoft.Extensions.Logging;
using StockMock.Repository;

namespace StockMock.Service.Areas
{
    public class BaseService<T>(ApplicationDbContext context, IMapper mapper, CancellationToken cancellationToken, ILogger<T> logger)
        where T : class
    {
        protected ApplicationDbContext _context = context;

        protected IMapper _mapper = mapper;

        protected CancellationToken _cancellationToken = cancellationToken;

        protected ILogger<T> _logger = logger;
    }
}
