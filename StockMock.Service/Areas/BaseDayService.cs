using AutoMapper;
using Microsoft.Extensions.Logging;
using StockMock.Data;
using StockMock.Service.Areas.Configs.Services;

namespace StockMock.Service.Areas
{
    public class BaseDayService<T>(
        ApplicationDbContext context, 
        IMapper mapper, 
        ILogger<T> logger, 
        DayService dayService) 
        : BaseService<T>(context, mapper, logger)
        where T : class
    {
        protected DayService _dayService = dayService;
    }
}
