using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StockMock.Application.Areas.Configs.Services;
using StockMock.Application.Areas.Mocks.Dtos;
using StockMock.Application.Extensions;
using StockMock.Domain.Common;
using StockMock.Domain.Common.Caches;
using StockMock.Domain.Entities.Mocks;
using StockMock.Infrastructure.Database;
using StockMock.Infrastructure.Extensions;

namespace StockMock.Application.Areas.Mocks.Services
{
    public class MockDateService : BaseBusService
    {
        public MockDateService(ApplicationDbContext context, IMapper mapper, CancellationToken cancellationToken, LocalCacheManager localCache, DayService dayService) : base(context, mapper, cancellationToken, localCache, dayService)
        {
        }

        private async Task ValidateAsync(MockDateDto dto)
        {
            MockDateDtoValidator validator = new MockDateDtoValidator();
            var validationResult = await validator.ValidateAsync(dto, _cancellationToken);

            if (!validationResult.IsValid)
                throw new BusinessExcption(validationResult.Errors.ToMessage());
        }

        public async Task<DateTime> AddMockDate(MockDateDto dto)
        {
            await ValidateAsync(dto);

            var mock = await _context.Mocks.FirstOrDefaultAsync(x => x.Id == dto.MockId);
            if (mock == null)
                throw new BusinessExcption("未找到对应的模拟股票操作数据");

            if (mock.Status != MockStatus.新建 || mock.Status != MockStatus.模拟中)
                throw new BusinessExcption("模拟股票操作数据状态不允许继续操作");

            if(!await _dayService.IsWorkDayAsync(dto.Date))
                throw new BusinessExcption("非工作日");

            var stock = await _context.Stocks.FirstOrDefaultAsync(e => e.Id == mock.StockId);
            if(stock == null)
                throw new BusinessExcption("未找到对应的股票");

            var stockDate = await _context.StockDates.FirstOrDefaultAsync(e => e.Date == dto.Date && e.StockId == stock.Id);
            if (stockDate == null)
                throw new BusinessExcption("未找到对应的股票日期");

            var preDay = await _dayService.GetPreWorkDayAsync(dto.Date);
            var preStockDate = await _context.StockDates.FirstOrDefaultAsync(e => e.StockId == stock.Id && e.Date == preDay);
            var preMockDate = await _context.MockDates.FirstOrDefaultAsync(e => e.MockId == mock.Id && e.Date == preDay);
            
            MockDate CreateNewMockDate()
            {
                MockDate entity = new MockDate()
                {
                    StockId = stock.Id,
                    StockName = stock.Name,
                    StockDateId = stockDate.Id,
                    MockId = mock.Id,
                    Date = dto.Date,
                    OpeningPrice = stockDate.OpeningPrice,
                    ClosingPrice = stockDate.ClosingPrice,
                    PreClosingPrice = preStockDate?.ClosingPrice,
                    PositionQuantity = dto.PositionQuantity,
                    PositionAmount = dto.PositionQuantity * stockDate.ClosingPrice,
                    PriceVariation = preStockDate != null ?  Math.Round((stockDate.ClosingPrice - preStockDate.ClosingPrice) / preStockDate.ClosingPrice * 100, 2) : stockDate.PriceVariation,
                    PositionRate = Math.Round(dto.PositionQuantity / mock.MaxPositionQuantity * 100m, 2),
                    PositionRateType = dto.PositionRateType!.Value,
                    Prediction = dto.Prediction!.Value,

                };

                var actualPrePredictionType = CalPredictionType();
            }
        }

        private PredictionType CalPredictionType(decimal priceVariation, int maxPriceVariation = 10)
        {
            var dicPredictionType = typeof(PredictionType).ToDictionary().OrderDescending();

            if (priceVariation >= maxPriceVariation * 1)
                return PredictionType.涨停;

            if (priceVariation >= maxPriceVariation * 0.7m)
                return PredictionType.大涨;

            if (priceVariation >= maxPriceVariation * 0.3m)
                return PredictionType.中涨;

            if (priceVariation >= maxPriceVariation * 0.1m)
                return PredictionType.小涨;

            if (priceVariation > maxPriceVariation * -0.1m)
                return PredictionType.微动;

            if (priceVariation > maxPriceVariation * -0.3m)
                return PredictionType.小跌;

            if (priceVariation > maxPriceVariation * -0.7m)
                return PredictionType.中跌;

            if (priceVariation > maxPriceVariation * -1)
                return PredictionType.大跌;

            return PredictionType.跌停;
        }
    }
}
