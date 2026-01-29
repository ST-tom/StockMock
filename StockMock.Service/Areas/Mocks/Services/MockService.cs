using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StockMock.Core.Mocks;
using StockMock.Core.Stocks;
using StockMock.Data;
using StockMock.Service.Areas.Configs.Services;
using StockMock.Service.Areas.Mocks.Dtos;
using StockMock.Service.FluentValidation;
using TS.Shared.Excption;
using TS.Shared.Extension;
using TS.Shared.Query;
using TS.Shared.User;

namespace StockMock.Service.Areas.Mocks.Services
{
    public class MockService(ApplicationDbContext context, IMapper mapper, ILogger<MockService> logger, DayService dayService, IUser user)
        : BaseDayService<MockService>(context, mapper, logger, dayService)
    {
        #region 字段

        private readonly IUser _user = user;

        #endregion

        #region 股票模拟

        #region 增删改查

        private static async Task ValidateAsync(MockDto dto, bool isRequiredId, CancellationToken cancellationToken)
        {
            MockDtoValidator validator = new(isRequiredId);
            var validationResult = await validator.ValidateAsync(dto, cancellationToken);

            if (!validationResult.IsValid)
                throw new BizException(validationResult.Errors.ToMessage());
        }

        public async Task AddAsync(MockDto dto, CancellationToken cancellationToken = default)
        {
            await ValidateAsync(dto, false, cancellationToken);

            var accountStock = await _context.AccountStocks.FirstOrDefaultAsync(e => e.StockCode == dto.StockCode && e.AccountId == _user.Id, cancellationToken) ?? throw new BizException("股票代码不存在或未添加");
            if (!_dayService.IsWorkDay(dto.MockDate))
                throw new BizException("非交易日，无法作为模拟起始日期");

            var stockDate = await _context.StockDates.FirstOrDefaultAsync(e => e.StockId == accountStock.Id && e.Date == dto.MockDate, cancellationToken) ?? throw new BizException("未找到对应股票日期数据，无法作为模拟起始日期");
            var mock = CreatNewMock();

            await _context.Mocks.AddAsync(mock, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            Mock CreatNewMock()
            {
                var newMock = new Mock()
                {
                    AccountId = _user.Id,
                    StockId = accountStock.StockId,
                    StockCode = accountStock.StockCode,
                    StockName = accountStock.StockName,
                    Status = MockStatus.created,
                    MockDate = dto.MockDate,
                };

                var (shares, amount) = CalMaxPositionQuantityAndAmount();
                newMock.MaxPositionQuantity = shares;
                newMock.BaseAmount = amount;
                newMock.LossLimitAmount = Math.Ceiling(amount * 0.2m);

                return newMock;
            }

            // 计算最大持仓数量和持仓金额
            (int shares, decimal amount) CalMaxPositionQuantityAndAmount()
            {
                int shares = (int)(AppConfig.mock_position_max_amount / stockDate.ClosingPrice / 100);

                var amountA = shares * 100 * stockDate.ClosingPrice;
                var amountB = (shares + 1) * 100 * stockDate.ClosingPrice;

                return Math.Abs(amountA - AppConfig.mock_position_max_amount) > Math.Abs(amountB - AppConfig.mock_position_max_amount) ? (shares + 1 * 100, amountB) : (shares, amountA);
            }
        }

        public async Task CancelAsync(MockDto dto, CancellationToken cancellationToken = default)
        {
            await ValidateAsync(dto, true, cancellationToken);
            var old = await _context.Mocks.FirstOrDefaultAsync(e => e.Id == dto.Id && e.AccountId == _user.Id, cancellationToken) ?? throw new BizException("该模拟数据不存在");
            if (old.Status == MockStatus.canceled)
                throw new BizException("该模拟数据已取消，无法重复取消");

            old.Status = MockStatus.canceled;
            _context.Mocks.Update(old);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task FinishAsync(MockDto dto, CancellationToken cancellationToken = default)
        {
            await ValidateAsync(dto, true, cancellationToken);
            var old = await _context.Mocks.FirstOrDefaultAsync(e => e.Id == dto.Id && e.AccountId == _user.Id, cancellationToken) ?? throw new BizException("该模拟数据不存在");
            if (old.Status == MockStatus.canceled)
                throw new BizException("该模拟数据已取消，无法置为完成");

            if (old.Status != MockStatus.finished)
            {
                old.EarningsRate = Math.Round(old.EarningsAmount / old.BaseAmount * 100, 2);

                old.Status = MockStatus.finished;
                _context.Mocks.Update(old);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task RestartAsync(MockDto dto, CancellationToken cancellationToken = default)
        {
            await ValidateAsync(dto, true, cancellationToken);
            var old = await _context.Mocks.FirstOrDefaultAsync(e => e.Id == dto.Id && e.AccountId == _user.Id, cancellationToken) ?? throw new BizException("该模拟数据不存在");
            if (old.Status != MockStatus.finished)
                throw new BizException("该模拟数据未完成，无法重新开始");

            old.Status = MockStatus.mocking;
            old.EarningsRate = 0;
            _context.Mocks.Update(old);
            await _context.SaveChangesAsync(cancellationToken);
        }

        #endregion

        #region 分页查询

        public async Task<PageList<Mock>> LoadAsync(MockPageDto pageDto, CancellationToken cancellationToken = default)
        {
            var validator = new MockPageDtoValidator();
            var validationResult = await validator.ValidateAsync(pageDto, cancellationToken);

            if (!validationResult.IsValid)
                throw new BizException(validationResult.Errors.ToMessage());

            pageDto.AccountId = _user.Id;

            var queryable = _context.Mocks.Where(pageDto.GetWhereLamda());
            var pageList = await pageDto.LoadAsync(queryable, cancellationToken);

            return pageList;
        }

        #endregion

        #region 股票模拟详情数据

        public async Task<MockInfoDto> LoadDetailAsync (MockDto dto, CancellationToken cancellationToken = default)
        {
            await ValidateAsync(dto, true, cancellationToken);

            MockInfoDto infoDto = new();
            var mock = await _context.Mocks.FirstOrDefaultAsync(e => e.Id == dto.Id && e.AccountId == _user.Id, cancellationToken) ?? throw new BizException("未找到对应的模拟股票操作数据");
            infoDto.Detail = _mapper.Map<MockInfoDetailDto>(mock);

            var days = _dayService.GetPreWorkDays();

            var mockDateQueryable = _context.MockDates.Where(e => e.MockId == mock.Id && e.AccountId == _user.Id && e.Date >= days.Min());

            infoDto.Days = await _mapper.ProjectTo<MockInfoDateDto>(mockDateQueryable).ToListAsync(cancellationToken);

            return infoDto;
        }

        #endregion

        #endregion

        #region 股票模拟日期

        #region 新增股票模拟日期数据

        private static async Task ValidateDateAsync(MockDateDto dto, CancellationToken cancellationToken)
        {
            MockDateDtoValidator validator = new();
            var validationResult = await validator.ValidateAsync(dto, cancellationToken);

            if (!validationResult.IsValid)
                throw new BizException(validationResult.Errors.ToMessage());
        }

        public async Task AddMockDate(MockDateDto dto, CancellationToken cancellationToken)
        {
            await ValidateDateAsync(dto, cancellationToken);

            var mock = await _context.Mocks.FirstOrDefaultAsync(e => e.Id == dto.MockId && e.AccountId == _user.Id, cancellationToken) ?? throw new BizException("未找到对应的模拟股票操作数据");
            if (mock.Status != MockStatus.created || mock.Status != MockStatus.mocking)
                throw new BizException("模拟股票操作数据状态不允许继续操作");

            if (!_dayService.IsWorkDay(dto.Date))
                throw new BizException("非工作日");

            var accountStock = await _context.AccountStocks.FirstOrDefaultAsync(e => e.StockId == mock.StockId && e.AccountId == _user.Id, cancellationToken) ?? throw new BizException("未找到对应的股票或未添加");
            var stockDate = await _context.StockDates.FirstOrDefaultAsync(e => e.Date == dto.Date && e.StockId == accountStock.StockId, cancellationToken) ?? throw new BizException("未找到对应的股票日期");
            var preDay = _dayService.GetPreWorkDay(dto.Date);
            var preStockDate = await _context.StockDates.FirstOrDefaultAsync(e => e.StockId == accountStock.StockId && e.Date == preDay, cancellationToken);
            var preMockDate = await _context.MockDates.FirstOrDefaultAsync(e => e.MockId == mock.Id && e.Date == preDay && e.AccountId == _user.Id, cancellationToken);

            var mockDate = CreateMockDate();
            await _context.MockDates.AddAsync(mockDate, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            MockDate CreateMockDate()
            {
                decimal positionRate = Math.Round(dto.PositionQuantity / mock.MaxPositionQuantity * 100m, 2);
                MockDate entity = new()
                {
                    StockId = accountStock.StockId,
                    StockName = accountStock.StockName,
                    StockDateId = stockDate.Id,
                    MockId = mock.Id,
                    Date = dto.Date,
                    OpeningPrice = stockDate.OpeningPrice,
                    ClosingPrice = stockDate.ClosingPrice,
                    PreClosingPrice = preStockDate?.ClosingPrice,
                    PositionQuantity = dto.PositionQuantity,
                    PositionAmount = dto.PositionQuantity * stockDate.ClosingPrice,
                    Gain = preStockDate != null ? Math.Round((stockDate.ClosingPrice - preStockDate.ClosingPrice) / preStockDate.ClosingPrice * 100, 2) : stockDate.Gain,
                    PositionRate = dto.PositionQuantity / mock.MaxPositionQuantity,
                    PositionType = CalPositionType(positionRate),
                    Prediction = dto.Prediction,
                };

                //存在昨日股票数据
                if (preStockDate != null)
                {
                    var actualPrePredictionType = CalPredictionType(entity.Gain, accountStock.Stock.BoardType.GetMaxGain());
                    entity.PredictionDeviationValue = Math.Abs(preMockDate!.Prediction - actualPrePredictionType);
                    entity.MockScore = entity.PositionRate * 4 * actualPrePredictionType.ToInt() + (entity.Gain > 0 ? entity.PredictionDeviationValue.Value : -entity.PredictionDeviationValue.Value);

                    mock.ScoreDataText = UpdateScoreDataText(mock.ScoreDataText, entity.MockScore!.Value);
                    _context.Mocks.Update(mock);
                }

                if (preMockDate != null)
                {
                    entity.ChangeQuantity = entity.PositionQuantity - preMockDate.PositionQuantity;
                    entity.TransactionCost = entity.ChangeQuantity * entity.ClosingPrice * accountStock.Stock.BoardType.GetCostRate();
                    entity.EarningsAmount = entity.PositionAmount - preMockDate.PositionAmount;

                    mock.PositionQuantity = entity.PositionQuantity;
                    mock.PositionAmount = mock.PositionAmount;
                    mock.EarningsAmount = mock.EarningsAmount + entity.EarningsAmount - entity.TransactionCost;
                    mock.EarningsRate = mock.EarningsAmount / mock.BaseAmount;

                    if(-mock.EarningsAmount > mock.LossLimitAmount)
                        throw new BizException("本次股票变更后，亏损金额大于最高补仓金额上限，不允许本次操作");

                    _context.Mocks.Update(mock);
                }

                return entity;
            }
        }

        /// <summary>
        /// 更新近30评分字符串
        /// </summary>
        /// <param name="scoreDataText"></param>
        /// <param name="todayScore"></param>
        /// <returns></returns>
        private static string UpdateScoreDataText(string scoreDataText, decimal todayScore)
        {
            if (string.IsNullOrWhiteSpace(scoreDataText))
                return todayScore.ToString();

            var scores = scoreDataText.TrySplit<string>(",").ToList();
            if (scores.Count >= 30)
                scores.RemoveRange(0, scores.Count - 29);

            scores.Add(todayScore.ToString());

            return scores.ToJoinString(",");
        }

        /// <summary>
        /// 计算涨幅        
        /// </summary>
        /// <param name="gain">涨幅</param>
        /// <param name="maxGain">最大涨幅</param>
        /// <returns></returns>
        private static PredictionType CalPredictionType(decimal gain, decimal maxGain = 10)
        {
            decimal[] coefficients = [1, 0.7m, 0.3m, 0.1m, -0.1m, -0.3m, -0.7m, -1];
            var dicPredictionType = typeof(PredictionType).ToDictionary().OrderDescending().ToDictionary();

            for (int i = 0; i < coefficients.Length; i++)
            {
                var coefficient = coefficients[i];
                if (coefficient > 0 ? gain >= maxGain * coefficient : gain > maxGain * coefficient)
                {
                    if (Enum.TryParse<PredictionType>(dicPredictionType[i].ToString(), out var predictionType))
                        return predictionType;
                }
            }
            return PredictionType.跌停;
        }

        /// <summary>
        /// 计算仓位类型
        /// </summary>
        /// <param name="positionRate"></param>
        /// <returns></returns>
        private static PositionType CalPositionType(decimal positionRate)
        {
            decimal[] coefficients = [0, 0.33m, 0.67m, 1];
            var dicPositionType = typeof(PositionType).ToDictionary().OrderByDescending(e => e.Key).ToDictionary();

            for (int i = 0; i < coefficients.Length; i++)
            {
                var coefficient = coefficients[i];
                if (positionRate <= coefficient)
                {
                    if (Enum.TryParse<PositionType>(dicPositionType[i].ToString(), out var positionRateType))
                        return positionRateType;
                }
            }
            return PositionType.空仓;
        }

        #endregion

        #region 加载模拟日期数据

        /// <summary>
        /// 加载最新模拟日期数据(最近30条)
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        public async Task<List<MockDate>> LoadLatestMockDatesAsync(MockDto dto, CancellationToken cancellationToken = default)
        {
            var validator = new MockDtoValidator(true);
            var validationResult = await validator.ValidateAsync(dto, cancellationToken);

            if (!validationResult.IsValid)
                throw new BizException(validationResult.Errors.ToMessage());

            return await _context.MockDates.Where(e => e.Id == _user.Id && e.MockId == dto.Id).OrderByDescending(e => e.Date).Take(30).ToListAsync(cancellationToken);
        }

        #endregion

        #region 模拟日期分页数据

        /// <summary>
        /// 加载模拟日期分页数据
        /// </summary>
        /// <param name="pageDto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        public async Task<PageList<MockDate>> LoadMockDateAsync(MockDatePageDto pageDto, CancellationToken cancellationToken = default)
        {
            var validator = new MockDatePageDtoValidator();
            var validationResult = await validator.ValidateAsync(pageDto, cancellationToken);

            if (!validationResult.IsValid)
                throw new BizException(validationResult.Errors.ToMessage());

            pageDto.AccountId = _user.Id;

            var queryable = _context.MockDates.Where(pageDto.GetWhereLamda());
            var pageList = await pageDto.LoadAsync(queryable, cancellationToken);

            return pageList;
        }

        #endregion

        #endregion
    }
}
