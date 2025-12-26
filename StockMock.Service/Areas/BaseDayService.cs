using AutoMapper;
using Microsoft.Extensions.Logging;
using StockMock.Repository;
using StockMock.Service.Areas.Configs.Services;

namespace StockMock.Service.Areas
{
    public class BaseDayService<T>(
        ApplicationDbContext context, 
        IMapper mapper, 
        CancellationToken cancellationToken,
        ILogger<T> logger, 
        DayService dayService) 
        : BaseService<T>(context, mapper, cancellationToken, logger)
        where T : class
    {
        protected DayService _dayService = dayService;
    }
}
