using System.ComponentModel;

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
        public bool IsEnabled { get; set; }

        public BoardType BoardType { get; set; }

        public List<StockDate> StockDateList { get; } = new List<StockDate>();

        public List<StockTime> StockTimeList { get; } = new List<StockTime>();
    }

    public enum BoardType
    {
        /// <summary>
        /// 主板
        /// </summary>
        [Description("主板")]
        MainBoard,
        /// <summary>
        /// 科创板
        /// </summary>
        [Description("科创板")]
        STARMarket,
        /// <summary>
        /// 创业板
        /// </summary>
        [Description("创业板")]
        ChiNextBoard,
        /// <summary>
        /// 北交所
        /// </summary>
        [Description("北交所")]
        BSE,
        /// <summary>
        /// 新三板基础层
        /// </summary>
        [Description("新三板基础层")]
        NEEQBasicLayer,
        /// <summary>
        /// 新三板创新层
        /// </summary>
        [Description("新三板创新层")]
        NEEQInnovationLayer,
    }
}
