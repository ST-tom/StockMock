using System.ComponentModel;

namespace StockMock.Core.Stocks
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

        /// <summary>
        /// 板块类型
        /// </summary>
        public BoardType BoardType { get; set; }

        public ICollection<StockDate> StockDateList { get; set; } = [];

        public ICollection<StockTime> StockTimeList { get; set; } = []; 
    }

    public enum BoardType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        NONE,
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

    public static class BoardTypeExtension
    {
        /// <summary>
        /// 最大涨幅
        /// </summary>
        /// <param name="boardType"></param>
        /// <returns></returns>
        public static decimal GetMaxGain(this BoardType boardType)
        {
            return boardType switch
            {
                BoardType.MainBoard => 10,
                BoardType.STARMarket => 20,
                BoardType.ChiNextBoard => 20,
                BoardType.BSE => 30,
                BoardType.NEEQInnovationLayer => 50,
                _ => 0,
            };
        }

        /// <summary>
        /// 成本费率
        /// </summary>
        /// <param name="boardType"></param>
        /// <returns></returns>
        public static decimal GetCostRate(this BoardType boardType)
        {
            return boardType switch
            {
                //BoardType.MainBoard => 10,
                //BoardType.STARMarket => 20,
                //BoardType.ChiNextBoard => 20,
                //BoardType.BSE => 30,
                //BoardType.NEEQInnovationLayer => 50,
                _ => (decimal)0.0005,
            };
        }
    }
}
