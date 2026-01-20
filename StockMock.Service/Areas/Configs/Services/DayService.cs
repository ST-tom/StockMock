using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StockMock.Core.Configs;
using StockMock.Data;
using StockMock.Data.Configs;
using StockMock.Service.Areas.Configs.Dtos;
using StockMock.Service.FluentValidation;
using TS.Shared.Excption;
using TS.Shared.Extension;
using TS.Shared.Query;
using TS.Shared.Util;

namespace StockMock.Service.Areas.Configs.Services
{
    public class DayService(
        ApplicationDbContext context,
        IMapper mapper,
        CancellationToken cancellationToken,
        ILogger<DayService> logger,
        WorkDayCache workDayCache
             ) : BaseService<DayService>(context, mapper, cancellationToken, logger)
    {
        private readonly WorkDayCache _workDayCache = workDayCache;

        #region 增删改查

        private async Task ValidateAsync(DayDto dto, bool isValidateName = false)
        {
            DayDtoValidator validator = new();
            var validationResult = await validator.ValidateAsync(dto, _cancellationToken);

            if (!validationResult.IsValid)
                throw new ApplicationExcption(validationResult.Errors.ToMessage());
        }

        public async Task AddAsync(DayDto dto)
        {
            await ValidateAsync(dto);

            var old = await _context.Days.FirstOrDefaultAsync(e => e.Date == dto.Date, _cancellationToken);
            if (old != null)
                throw new ApplicationExcption("该日期已存在，请勿重复添加");

            _context.Days.Add(_mapper.Map<Day>(dto));
            await _context.SaveChangesAsync(_cancellationToken);
            await _workDayCache.RefreshAllAsync();
        }

        public async Task<DayDto> GetAsync(long id)
        {
            if (id <= 0)
                throw new ApplicationExcption("Id不合法");

            var old = await _context.Days.FindAsync(id, _cancellationToken);

            return old == null ? throw new ApplicationExcption("该日期不存在") : _mapper.Map<DayDto>(old);
        }

        public async Task UpdateAsync(DayDto dto)
        {
            await ValidateAsync(dto);

            var old = await _context.Days.FirstOrDefaultAsync(e => e.Date == dto.Date, _cancellationToken) ?? throw new ApplicationExcption("该日期不存在，请先添加");
            if (old.IsWorkDay != dto.IsWorkDay)
            {
                old.IsWorkDay = dto.IsWorkDay;

                _context.Days.Update(old);
                await _context.SaveChangesAsync(_cancellationToken);
                await _workDayCache.RefreshAllAsync();
            }
        }

        #endregion

        #region 批量生成当前日期的全年数据

        /// <summary>
        /// 批量生成当前日期的全年数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationExcption"></exception>
        public async Task BuildYearDaysAsync(DayDto dto)
        {
            await ValidateAsync(dto);

            var year = dto.Date.Year;
            var dayDic = await _context.Days.Where(e => e.Date.Year == year).ToDictionaryAsync(e => e.Date, _cancellationToken);

            if (dayDic.Count == 0)
                throw new ApplicationExcption("该年份没有节日数据，请先添加特殊日期数据");

            var length = DateTime.IsLeapYear(year) ? 366 : 365;
            if (dayDic.Count == length)
                throw new ApplicationExcption("该年份已有日期数据，请勿重复添加");

            var date = new DateOnly(year, 1, 1);
            for (int i = 0; i < length - 1; i++)
            {
                if (!dayDic.ContainsKey(date))
                    await _context.Days.AddAsync(
                        new Day
                        {
                            Date = date,
                            IsWorkDay = date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday
                        });

                date = date.AddDays(1);
            }

            await _context.SaveChangesAsync(_cancellationToken);
        }

        #endregion

        #region 分页查询

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageDto"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationExcption"></exception>
        public async Task<PageList<Day>> LoadAsync(DayPageDto pageDto)
        {
            var validator = new DayPageDtoValidator();
            var validationResult = await validator.ValidateAsync(pageDto, _cancellationToken);

            if (!validationResult.IsValid)
                throw new ApplicationExcption(validationResult.Errors.ToMessage());

            var queryable = _context.Days.Where(pageDto.GetWhereLamda());
            var pageList = await pageDto.LoadAsync(queryable, _cancellationToken);

            return pageList;
        }

        #endregion

        #region 工作日相关
        /// <summary>
        /// 判断是否是工作日
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool IsWorkDay(DateOnly date)
        {
            var day = _workDayCache.Get(date);
            if (day == null)
                return false;

            return !day.IsWorkDay;
        }

        /// <summary>
        /// 判断是否是工作日
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool IsWorkDay(DateTime date) => IsWorkDay(date.ToDateOnly());

        /// <summary>
        /// 获取指定日期的前一个工作日
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationExcption"></exception>
        public DateOnly GetPreWorkDay(DateOnly date)
        {
            int count = 0;
            Day? preDay;
            do
            {
                preDay = _workDayCache.Get(date);
                date.AddDays(-1);
            } while (count < 14 && preDay != null);

            if (preDay == null)
                throw new ApplicationExcption("该日期不存在，请先添加或者非工作日");

            return preDay.Date;
        }

        #endregion
    }
}
