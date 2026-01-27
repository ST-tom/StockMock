using Microsoft.EntityFrameworkCore;
using StockMock.Core.Configs;
using TS.Shared.MemoryCache;
using TS.Shared.Util;

namespace StockMock.Data.Configs
{
    /// <summary>
    /// 最近2年日期缓存
    /// </summary>
    /// <param name="dbContext"></param>
    public class DayCache(ApplicationDbContext dbContext) : BasAllObjectMemoryCache<Day, DateOnly>()
    {
        private readonly ApplicationDbContext dbContext = dbContext;

        protected override Func<CancellationToken, Task<IEnumerable<Day>>> QueryDataFuncAsync => async (cancellationToken) => await dbContext.Days.Where(e => e.Date >= DateTimeUtil.GetLastYear()).ToListAsync(cancellationToken);
            
        protected override Func<Day, DateOnly>? GetKeyFunc => (e) => e.Date;
    }
}
