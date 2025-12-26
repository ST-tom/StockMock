using StockMock.Core;

namespace StockMock.Core.Stocks
{
    /// <summary>
    /// 股票日数据
    /// </summary>
    public class StockDate : BaseEntity
    {
        /// <summary>
        /// 股票id
        /// </summary>
        public long StockId { get; set; }

        /// <summary>
        /// 股票代码
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// 股票名称
        /// </summary>
        public string StockName { get; set; }

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
        public decimal ClosingPrice { get; set; }

        /// <summary>
        /// 昨日收盘价
        /// </summary>
        public decimal? PreClosingPrice { get; set; }

        /// <summary>
        /// 股票涨幅
        /// </summary>
        public decimal Gain { get; set; }

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

        /// <summary>
        /// 是否ST
        /// </summary>
        public bool IsST { get; set; }

        public Stock Stock { get; set; }
    }
}
