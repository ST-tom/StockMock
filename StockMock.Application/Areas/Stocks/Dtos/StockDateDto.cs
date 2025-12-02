using FluentValidation;
using StockMock.Application.Extensions;

namespace StockMock.Application.Areas.Stocks.Dtos
{
    public class StockDateDto
    {
        /// <summary>
        /// 编号
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// 股票代码
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateOnly Date { get; set; }

        /// <summary>
        /// 开盘价
        /// </summary>
        public decimal OpeningPrice { get; set; }

        /// <summary>
        /// 收盘价
        /// </summary>
        public decimal? ClosingPrice { get; set; }

        /// <summary>
        /// 昨日收盘价
        /// </summary>
        public decimal? PreClosingPrice { get; set; }

        /// <summary>
        /// 股票涨幅
        /// </summary>
        public decimal? PriceVariation { get; set; }

        /// <summary>
        /// 成交量
        /// </summary>
        public long? TradingVolume { get; set; }

        /// <summary>
        /// 成交总金额
        /// </summary>
        public decimal? Turnover { get; set; }

        /// <summary>
        /// 换手率
        /// </summary>
        public decimal? TurnoverRate { get; set; }

        public StockDateDto()
        {

        }
    }

    public class StockDateDtoValidator : AbstractValidator<StockDateDto>
    {
        public StockDateDtoValidator(bool isRequireId = false)
        {
            if (!isRequireId)
            {
                RuleFor(v => v.StockCode)
                    .MustStockCode();
            }
            else
            {
                RuleFor(v => v.Id)
                    .MustId();
                RuleFor(v => v.StockCode)
                    .MustStockCodeLength();
            }

            RuleFor(v => v.Date)
                .MustDateRange();

            RuleFor(v => v.PriceVariation)
                .MustPriceVariationRange();
        }
    }
}
