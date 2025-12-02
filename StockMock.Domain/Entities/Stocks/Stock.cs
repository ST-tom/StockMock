namespace StockMock.Domain.Entities.Stocks
{
    /// <summary>
    /// 股票
    /// </summary>
    public class Stock : BaseEntity
    {
        /// <summary>
        /// 股票代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 股票名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        public string 

        public List<StockDate> StockDateList { get; } = new List<StockDate>();

        public List<StockTime> StockTimeList { get; } = new List<StockTime>();
    }
}
