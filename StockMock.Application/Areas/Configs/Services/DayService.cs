using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StockMock.Application.Areas.Configs.Dtos;
using StockMock.Application.Base;
using StockMock.Application.Extensions;
using StockMock.Domain.Common;
using StockMock.Domain.Common.Caches;
using StockMock.Domain.Entities.Configs;
using StockMock.Infrastructure.Database;
using StockMock.Infrastructure.Extensions;
using StockMock.Infrastructure.Utils;

namespace StockMock.Application.Areas.Configs.Services
{
    public class DayService : BaseService
    {
        public const string cacheKeyWorkDay = "configs.day.workday";

        public DayService(ApplicationDbContext context, IMapper mapper, CancellationToken cancellationToken, LocalCacheManager localCache) : base(context, mapper, cancellationToken, localCache)
        {

        }

        private async Task ValidateAsync(DayDto dto, bool isValidateName = false)
        {
            DayDtoValidator validator = new DayDtoValidator();
            var validationResult = await validator.ValidateAsync(dto, _cancellationToken);

            if (!validationResult.IsValid)
                throw new BusinessExcption(validationResult.Errors.ToMessage());
        }

        public async Task AddAsync(DayDto dto)
        {
            await ValidateAsync(dto);

            var old = await _context.Days.FirstOrDefaultAsync(e => e.Date == dto.Date, _cancellationToken);
            if (old != null)
                throw new BusinessExcption("该日期已存在，请勿重复添加");

            _context.Days.Add(_mapper.Map<Day>(dto));
            await _context.SaveChangesAsync(_cancellationToken);
            await InitCacheAsync();
        }

        public async Task<DayDto> GetAsync(long id)
        {
            if (id <= 0)
                throw new BusinessExcption("Id不合法");

            var old = await _context.Days.FindAsync(id, _cancellationToken);

            if (old == null)
                throw new BusinessExcption("该日期不存在");

            return _mapper.Map<DayDto>(old);
        }

        public async Task UpdateAsync(DayDto dto)
        {
            await ValidateAsync(dto);

            var old = await _context.Days.FirstOrDefaultAsync(e => e.Date == dto.Date, _cancellationToken);
            if (old == null)
                throw new BusinessExcption("该日期不存在，请先添加");

            if (old.IsWorkDay != dto.IsWorkDay)
            {
                old.IsWorkDay = dto.IsWorkDay;

                _context.Days.Update(old);
                await _context.SaveChangesAsync(_cancellationToken);
                await InitCacheAsync();
            }
        }

        public async Task BuildYearDaysAsync(DayDto dto)
        {
            await ValidateAsync(dto);

            var year = dto.Date.Year;
            var dayDic = await _context.Days.Where(e => e.Date.Year == year).ToDictionaryAsync(e => e.Date, _cancellationToken);

            if (dayDic.Count == 0)
                throw new BusinessExcption("该年份没有节日数据，请先添加特殊日期数据");

            var length = DateTime.IsLeapYear(year) ? 366 : 365;
            if (dayDic.Count == length)
                throw new BusinessExcption("该年份已有日期数据，请勿重复添加");

            var date = new DateOnly(year, 1, 1);
            for (int i = 0; i < length - 1; i++)
            {
                if (!dayDic.ContainsKey(date))
                    await _context.Days.AddAsync(
                        new Day
                        {
                            Date = date,
                            IsWorkDay = (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) ? false : true
                        });

                date = date.AddDays(1);
            }

            await _context.SaveChangesAsync(_cancellationToken);
            await InitCacheAsync();
        }

        public async Task<PageList<Day>> LoadAsync(DayPageDto pageDto)
        {
            var validator = new DayPageDtoValidator();
            var validationResult = await validator.ValidateAsync(pageDto, _cancellationToken);

            if (!validationResult.IsValid)
                throw new BusinessExcption(validationResult.Errors.ToMessage());

            var queryable = _context.Days.Where(pageDto.GetWhereLamda());
            var pageList = await pageDto.LoadAsync(queryable, _cancellationToken);

            return pageList;
        }

        public async Task InitCacheAsync()
        {
            var days = await _context.Days.Where(e => e.Date > DateTimeUtil.GetLastYear() && e.IsWorkDay).ToListAsync(_cancellationToken);
            _loaclCache.Set(cacheKeyWorkDay, days);
        }

        public async ValueTask<List<Day>> GetWorkDayListAsync()
        {
            var days = _loaclCache.Get<List<Day>>(cacheKeyWorkDay);
            if (days == null)
                await InitCacheAsync();

            if (days == null)
                days = new List<Day>();

            return days;
        }

        public async ValueTask<bool> IsWorkDayAsync(DateOnly date)
        {
            var days = await GetWorkDayListAsync();
            var day = days.FirstOrDefault(e => e.Date == date);
            if (day == null)
                throw new BusinessExcption("该日期不存在，请先添加");

            return !day.IsWorkDay;
        }

        public async ValueTask<bool> IsWorkDay(DateTime date)
        {
            var days = await GetWorkDayListAsync();
            var day = days.FirstOrDefault(e => e.Date == date.ToDateOnly());
            if (day == null)
                throw new BusinessExcption("该日期不存在，请先添加");

            return !day.IsWorkDay;
        }

        public async ValueTask<DateOnly> GetPreWorkDayAsync(DateOnly date)
        {
            var days = await GetWorkDayListAsync();
            var dayIndex = days.FindIndex(e => e.Date == date);
            if (dayIndex == -1)
                throw new BusinessExcption("该日期不存在，请先添加");

            var preDay = days[dayIndex - 1];
            return preDay.Date;
        }
    }
}
