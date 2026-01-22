using AutoMapper;
using Microsoft.Extensions.Logging;
using StockMock.Data;

namespace StockMock.Service.Areas
{
    public class BaseService<T>(ApplicationDbContext context, IMapper mapper, ILogger<T> logger)
        where T : class
    {
        protected readonly ApplicationDbContext _context = context;

        protected readonly IMapper _mapper = mapper;

        protected readonly ILogger<T> _logger = logger;
    }
}
