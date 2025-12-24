using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StockMock.Application.Areas.Configs.Services;
using StockMock.Application.Areas.Stocks.Dtos;
using StockMock.Application.Base;
using StockMock.Application.Common;
using StockMock.Application.Extensions;
using StockMock.Domain.Common;
using StockMock.Domain.Common.Caches;
using StockMock.Domain.Entities.Stocks;
using StockMock.Infrastructure.Database;
using StockMock.Infrastructure.Extensions;
using StockMock.Infrastructure.Utils;

namespace StockMock.Application.Areas.AccountStocks.Services
{
    public class StockDateService(
        ApplicationDbContext context, 
        IMapper mapper, 
        CancellationToken cancellationToken, 
        LocalCacheManager localCache, 
        Logger<StockDateService> logger, 
        DayService dayService) 
        : BaseDayService<StockDateService>(context, mapper, cancellationToken, localCache, logger, dayService)
    {
        #region 增删改查

        private async Task ValidateAsync(StockDateDto dto, bool isRequiredId = true)
        {
            StockDateDtoValidator validator = new StockDateDtoValidator(isRequiredId);
            var validationResult = await validator.ValidateAsync(dto, _cancellationToken);

            if (!validationResult.IsValid)
                throw new ApplicationExcption(validationResult.Errors.ToMessage());
        }

        public async Task AddAsync(StockDateDto dto)
        {
            await ValidateAsync(dto, false);

            var old = await _context.StockDates.FirstOrDefaultAsync(e => e.StockCode == dto.StockCode && e.Date == dto.Date, _cancellationToken);
            if (old != null)
                throw new ApplicationExcption("该股票在该日期的行情数据已存在，请勿重复添加");

            var isWorkDay = await _dayService.IsWorkDayAsync(dto.Date);
            if (!isWorkDay)
                throw new ApplicationExcption("非工作日，无法添加行情数据");

            if (!dto.ClosingPrice.HasValue || !dto.PriceVariation.HasValue || !dto.PreClosingPrice.HasValue)
            {
                StockDate? predyesterday = null;
                if (!dto.PriceVariation.HasValue)
                    predyesterday = await _context.StockDates.OrderByDescending(e => e.Date).FirstOrDefaultAsync(e => e.StockCode == dto.StockCode && e.Date < dto.Date, _cancellationToken);

                if (predyesterday == null)
                    throw new ApplicationExcption("该股票在该日期之前的行情数据不存在，无法计算缺失数据");

                if (!dto.ClosingPrice.HasValue)
                {
                    if (dto.PriceVariation.HasValue)
                        dto.ClosingPrice = predyesterday.ClosingPrice * (1 + dto.PriceVariation!.Value);
                    else
                        throw new ApplicationExcption("收盘价未录入或无法计算得出");
                }

                if (!dto.PreClosingPrice.HasValue)
                {
                    if (predyesterday != null)
                        dto.PreClosingPrice = predyesterday.ClosingPrice;
                    else if (dto.ClosingPrice.HasValue && dto.PriceVariation.HasValue)
                        dto.PreClosingPrice = dto.ClosingPrice / (1 + dto.PriceVariation!.Value);
                }

                if (!dto.PriceVariation.HasValue)
                {
                    if (dto.ClosingPrice.HasValue && dto.PreClosingPrice.HasValue)
                        dto.PriceVariation = (dto.ClosingPrice.Value - dto.PreClosingPrice.Value) / dto.PreClosingPrice.Value;
                    else
                        throw new ApplicationExcption("收盘价或昨日收盘价未录入，无法计算得出");
                }

                await ValidateAsync(dto, false);
            }

            var stockDate = _mapper.Map<StockDate>(dto);
            await _context.StockDates.AddAsync(stockDate, _cancellationToken);
            await _context.SaveChangesAsync(_cancellationToken);
        }

        public async Task<StockDateDto> GetAsync(long Id)
        {
            if (Id <= 0)
                throw new ApplicationExcption("Id不合法");

            var old = await _context.StockDates.FindAsync(Id, _cancellationToken);
            if (old == null)
                throw new ApplicationExcption("没有找到该行情数据");

            return _mapper.Map<StockDateDto>(old);
        }

        public async Task UpdateAsync(StockDateDto dto)
        {
            await ValidateAsync(dto, false);

            var old = await _context.StockDates.FindAsync(dto.Id, _cancellationToken);
            if (old == null)
                throw new ApplicationExcption("该股票在该日期的行情数据不存在，无法删除");

            old = _mapper.Map(dto, old);
            _context.StockDates.Update(old);
            await _context.SaveChangesAsync(_cancellationToken);
        }

        public async Task DeleteAsync(string idText)
        {
            if (string.IsNullOrWhiteSpace(idText))
                throw new ApplicationExcption("没有需要删除的行情数据");

            var ids = idText.TrySplit<long>();
            var olds = await _context.StockDates.Where(e => ids.Contains(e.Id)).ToListAsync(_cancellationToken);
            if (!olds.Any())
                throw new ApplicationExcption("没有找到要删除的行情数据");

            _context.StockDates.RemoveRange(olds);
            await _context.SaveChangesAsync(_cancellationToken);
        }

        #endregion

        #region 分页查询

        public async Task<PageList<StockDate>> LoadAsync(StockDatePageDto pageDto)
        {
            var validator = new StockDatePageDtoValidator();
            var validationResult = await validator.ValidateAsync(pageDto, _cancellationToken);

            if (!validationResult.IsValid)
                throw new ApplicationExcption(validationResult.Errors.ToMessage());

            var queryable = _context.StockDates.Where(pageDto.GetWhereLamda());
            var pageList = await pageDto.LoadAsync(queryable, _cancellationToken);

            return pageList;
        }

        #endregion

        #region 导入数据

        public async Task ImportAsync(Stream stream, string fileName)
        {
            IEnumerable<StockDateDto>? rows = default;
            try
            {
                rows = await ExcelUtil.ReadExcelAsync<StockDateDto>(stream, fileName, cancellationToken: _cancellationToken);
                if (rows == null || !rows.Any())
                    throw new ApplicationExcption("没有找到有效数据");
            }
            catch (Exception ex)
            {
                throw new ApplicationExcption(ex.Message);
            }

            if (rows.Count() > 2000)
                throw new ApplicationExcption("一次性导入数据不能超过2000条");

            var lamda = LinqKit.PredicateBuilder.New<StockDate>(true);

            rows.ForEach(row =>
            {
                lamda.And(e => e.StockCode == row.StockCode && e.Date == row.Date);
            });
            var olds = await _context.StockDates.Where(lamda).ToListAsync(_cancellationToken);

            ImportStringBuilder importSb = new ImportStringBuilder();
            for (int index = 0; index < rows.Count(); index++)
            {
                importSb.AppendNewLine($"第{index + 2}行数据验证失败：");
                var row = rows.ElementAt(index);

                if (!await ValidateAsync())
                    continue;

                var old = olds.Find(e => e.StockCode == row.StockCode && e.Date == row.Date);
                if (old != null)
                {
                    importSb.AppendErrmsgAndEndLine("该股票在该日期的行情数据已存在，请勿重复添加");
                    continue;
                }

                var isWorkDay = await _dayService.IsWorkDayAsync(row.Date);
                if (!isWorkDay)
                {
                    importSb.AppendErrmsgAndEndLine("非工作日，无法添加行情数据");
                    continue;
                }

                if (!row.ClosingPrice.HasValue)
                    importSb.AppendErrmsg("收盘价未录入");

                if (!row.PriceVariation.HasValue)
                    importSb.AppendErrmsg("涨跌幅未录入");

                if (!row.PreClosingPrice.HasValue)
                    row.PreClosingPrice = row.ClosingPrice / (1 + row.PriceVariation!.Value);

                var stockDate = _mapper.Map<StockDate>(row);
                _context.StockDates.Add(stockDate);

                async Task<bool> ValidateAsync()
                {
                    StockDateDtoValidator validator = new StockDateDtoValidator(false);
                    var validationResult = await validator.ValidateAsync(row, _cancellationToken);

                    if (!validationResult.IsValid)
                    {
                        importSb.AppendErrmsgAndEndLine(validationResult.Errors.ToMessage());
                        return false;
                    }

                    return true;
                }
            }

            await _context.SaveChangesAsync(_cancellationToken);
        }

        #endregion

    }
}
