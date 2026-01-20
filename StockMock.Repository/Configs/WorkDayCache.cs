using Microsoft.EntityFrameworkCore;
using StockMock.Core.Configs;
using TS.Shared.MemoryCache;
using TS.Shared.Util;

namespace StockMock.Data.Configs
{
    public class WorkDayCache(ApplicationDbContext dbContext) : BasAllMemoryCache<DateOnly, Day>()
    {
        private readonly ApplicationDbContext dbContext = dbContext;

        protected override Func<CancellationToken, Task<IEnumerable<Day>>> QueryDataFuncAsync => async (cancellationToken) => await dbContext.Days.Where(e => e.Date > DateTimeUtil.GetLastYear() && e.IsWorkDay).ToListAsync(cancellationToken);
            
        protected override Func<Day, DateOnly>? GetKeyFunc => (e) => e.Date;
    }
}
