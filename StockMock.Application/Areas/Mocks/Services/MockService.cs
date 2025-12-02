using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StockMock.Application.Areas.Configs.Services;
using StockMock.Application.Areas.Mocks.Dtos;
using StockMock.Application.Base;
using StockMock.Application.Extensions;
using StockMock.Domain.Common;
using StockMock.Domain.Common.Caches;
using StockMock.Domain.Entities.Mocks;
using StockMock.Infrastructure.Database;

namespace StockMock.Application.Areas.Mocks.Services
{
    public class MockService : BaseBusService
    {
        public const decimal baseAmount = 100000;

        public MockService(ApplicationDbContext context, IMapper mapper, CancellationToken cancellationToken, LocalCacheManager localCache, DayService dayService) : base(context, mapper, cancellationToken, localCache, dayService)
        {
        }

        private async Task ValidateAsync(MockDto dto, bool isRequiredId = false)
        {
            MockDtoValidator validator = new MockDtoValidator(isRequiredId);
            var validationResult = await validator.ValidateAsync(dto, _cancellationToken);

            if (!validationResult.IsValid)
                throw new BusinessExcption(validationResult.Errors.ToMessage());
        }

        public async Task AddAsync(MockDto dto)
        {
            await ValidateAsync(dto);

            var stock = await _context.Stocks.FirstOrDefaultAsync(e => e.Code == dto.StockCode, _cancellationToken);
            if (stock == null)
                throw new BusinessExcption("股票代码不存在");

            if (!await _dayService.IsWorkDayAsync(dto.MockDate))
                throw new BusinessExcption("非交易日，无法作为模拟起始日期");

            var stockDate = await _context.StockDates.FirstOrDefaultAsync(e => e.StockId == stock.Id && e.Date == dto.MockDate, _cancellationToken);
            if (stockDate == null)
                throw new BusinessExcption("未找到对应股票日期数据，无法作为模拟起始日期");

            var mock = CreatNewMock();

            await _context.Mocks.AddAsync(mock, _cancellationToken);
            await _context.SaveChangesAsync(_cancellationToken);

            Mock CreatNewMock()
            {
                var newMock = new Mock()
                {
                    StockId = stock.Id,
                    StockCode = stock.Code,
                    StockName = stock.Name,
                    Status = MockStatus.created,
                    MockDate = dto.MockDate,
                };

                var calResult = CalMaxPositionQuantityAndAmount();
                newMock.MaxPositionQuantity = calResult.shares;
                newMock.BaseAmount = calResult.amount;
                newMock.ToppingUpAmount = Math.Ceiling(calResult.amount * 1.2m);

                return newMock;
            }

            (int shares, decimal amount) CalMaxPositionQuantityAndAmount()
            {
                int shares = (int)(baseAmount / stockDate.ClosingPrice / 100);

                var amountA = shares * 100 * stockDate.ClosingPrice;
                var amountB = (shares + 1) * 100 * stockDate.ClosingPrice;

                return Math.Abs(amountA - baseAmount) > Math.Abs(amountB - baseAmount) ? ((shares + 1 * 100), amountB) : (shares, amountA);
            }
        }

        public async Task CancelAsync(MockDto dto)
        {
            await ValidateAsync(dto, true);
            var old = await _context.Mocks.FirstOrDefaultAsync(e => e.Id == dto.Id, _cancellationToken);
            if (old == null)
                throw new BusinessExcption("该模拟数据不存在");

            if (old.Status == MockStatus.canceled)
                throw new BusinessExcption("该模拟数据已取消，无法重复取消");

            dto.Status = MockStatus.canceled;
            old.Status = dto.Status;
            _context.Mocks.Update(old);
            await _context.SaveChangesAsync(_cancellationToken);
        }

        public async Task FinishAsync(MockDto dto)
        {
            await ValidateAsync(dto, true);
            var old = await _context.Mocks.FirstOrDefaultAsync(e => e.Id == dto.Id, _cancellationToken);
            if (old == null)
                throw new BusinessExcption("该模拟数据不存在");

            if(old.Status == MockStatus.canceled)
                throw new BusinessExcption("该模拟数据已取消，无法置为完成");

            dto.Status = MockStatus.finished;
            if (old.Status != dto.Status)
            {
                old.Status = dto.Status;
                _context.Mocks.Update(old);
                await _context.SaveChangesAsync(_cancellationToken);
            }
        }

        public async Task RestartAsync(MockDto dto)
        {
            await ValidateAsync(dto, true);
            var old = await _context.Mocks.FirstOrDefaultAsync(e => e.Id == dto.Id, _cancellationToken);
            if (old == null)
                throw new BusinessExcption("该模拟数据不存在");

            if (old.Status != MockStatus.finished)
                throw new BusinessExcption("该模拟数据未完成，无法重新开始");

            dto.Status = MockStatus.mocking;
            old.Status = dto.Status;
            _context.Mocks.Update(old);
            await _context.SaveChangesAsync(_cancellationToken);
        }

        public async Task<PageList<Mock>> LoadAsync(MockPageDto pageDto)
        {
            var validator = new MockPageDtoValidator();
            var validationResult = await validator.ValidateAsync(pageDto, _cancellationToken);

            if (!validationResult.IsValid)
                throw new BusinessExcption(validationResult.Errors.ToMessage());

            var queryable = _context.Mocks.Where(pageDto.GetWhereLamda());
            var pageList = await pageDto.LoadAsync(queryable, _cancellationToken);

            return pageList;
        }
    }
}
