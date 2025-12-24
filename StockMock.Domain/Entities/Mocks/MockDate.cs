using StockMock.Domain.Entities.Stocks;

namespace StockMock.Domain.Entities.Mocks
{
    public class MockDate : BaseEntity
    {
        /// <summary>
        /// 股票代码
        /// </summary>
        public long StockId { get; set; }

        /// <summary>
        /// 股票名称
        /// </summary>
        public string StockName { get; set; }

        /// <summary>
        /// 股票日期数据编号
        /// </summary>
        public long StockDateId { get; set; }

        /// <summary>
        /// Mock编号
        /// </summary>
        public long MockId { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateOnly Date { get; set; }

        /// <summary>
        /// 开盘价
        /// </summary>
        public decimal OpeningPrice { get; set; }

        /// <summary>
        /// 昨日收盘价
        /// </summary>
        public decimal? PreClosingPrice { get; set; }

        /// <summary>
        /// 收盘价
        /// </summary>
        public decimal ClosingPrice { get; set; }

        /// <summary>
        /// 持仓数量
        /// </summary>
        public int PositionQuantity { get; set; }

        /// <summary>
        /// 变更数量
        /// </summary>
        public decimal? ChangeQuantity { get; set; }

        /// <summary>
        /// 昨日持仓金额
        /// </summary>
        public decimal? PrePositionAmount { get; set; }

        /// <summary>
        /// 持仓金额
        /// </summary>
        public decimal PositionAmount { get; set; }

        /// <summary>
        /// 盈亏金额（昨日盘尾操作后根据今日涨幅计算）
        /// </summary>
        public decimal EarningsAmount { get; set; }

        /// <summary>
        /// 交易成本
        /// </summary>
        public decimal TransactionCost { get; set; }

        /// <summary>
        /// 股票涨幅
        /// </summary>
        public decimal Gain { get; set; }

        /// <summary>
        /// 仓位比例
        /// </summary>
        public decimal PositionRate { get; set; }

        /// <summary>
        /// 仓位比例类型
        /// </summary>
        public PositionRateType PositionRateType { get; set; }

        /// <summary>
        /// 预测涨幅
        /// </summary>
        public PredictionType Prediction { get; set; }

        /// <summary>
        /// 预测偏离度
        /// </summary>
        public int? PredictionDeviationValue { get; set; }

        /// <summary>
        /// 预测评分
        /// </summary>
        public decimal? MockScore { get; set; }

        public Stock Stock { get; set; }

        public StockDate StockDate { get; set; }

        public Mock Mock { get; set; }

    }

    public enum PositionRateType
    {
        空仓 = 0,
        轻仓 = 1,
        中仓 = 2,
        重仓 = 3,
        满仓 = 4,
    }

    public enum PredictionType
    {
        涨停 = 4,
        大涨 = 3,
        中涨 = 2,
        小涨 = 1,
        微动 = 0,
        小跌 = -1,
        中跌 = -2,
        大跌 = -3,
        跌停 = -4,
        无法预测 = -100,
    }
}
