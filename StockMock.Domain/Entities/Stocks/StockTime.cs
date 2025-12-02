namespace StockMock.Domain.Entities.Stocks
{
    /// <summary>
    /// 股票时间数据
    /// </summary>
    public class StockTime : BaseEntity
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
        /// 交易时间
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 成交量
        /// </summary>
        public long TradingVolume { get; set; }

        /// <summary>
        /// 成交总金额
        /// </summary>
        public decimal Turnover { get; set; }

        public Stock Stock { get; set; }

    }
}
