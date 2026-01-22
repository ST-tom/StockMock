using StockMock.Core.Mocks;

namespace StockMock.Service.Areas.Mocks.Dtos
{
    public class MockInfoDto
    {
        public MockInfoDetailDto Detail { get; set; }

        public List<MockInfoDateDto> Days { get; set; } = [];
    }

    public class MockInfoDetailDto
    {
        /// <summary>
        /// 股票代码
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// 股票名称
        /// </summary>
        public string StockName { get; set; }

        /// <summary>
        /// 模拟状态
        /// </summary>
        public MockStatus Status { get; set; }

        /// <summary>
        /// 模拟起始日期
        /// </summary>
        public DateOnly MockDate { get; set; }

        /// <summary>
        /// 模拟天数
        /// </summary>
        public int MockDays { get; set; }

        /// <summary>
        /// 基准金额
        /// </summary>
        public decimal BaseAmount { get; set; }

        /// <summary>
        /// 亏损上限金额
        /// </summary>
        public decimal LossLimitAmount { get; set; }

        /// <summary>
        /// 最大持仓数量
        /// </summary>
        public int MaxPositionQuantity { get; set; }

        /// <summary>
        /// 持仓数量
        /// </summary>
        public int PositionQuantity { get; set; }

        /// <summary>
        /// 持仓金额
        /// </summary>
        public decimal PositionAmount { get; set; }

        /// <summary>
        /// 盈亏金额
        /// </summary>
        public decimal EarningsAmount { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 收益率
        /// </summary>
        public decimal EarningsRate { get; set; }

        /// <summary>
        /// 近30天预测评分数据字符串,隔开的评分字符串
        /// </summary>
        public string ScoreDataText { get; set; }

        /// <summary>
        /// 近10天平均预测评分
        /// </summary>
        public decimal? ScoreDataFor10 { get; set; }

        /// <summary>
        /// 近20天平均预测评分
        /// </summary>
        public decimal? ScoreDataFoo20 { get; set; }

        /// <summary>
        /// 近30天平均预测评分
        /// </summary>
        public decimal? ScoreDataFoo30 { get; set; }
    }

    public class MockInfoDateDto
    {
        /// <summary>
        /// 日期
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// 当日开盘价
        /// </summary>
        public decimal OpeningPrice { get; set; }

        /// <summary>
        /// 当日收盘价
        /// </summary>
        public decimal ClosingPrice { get; set; }

        /// <summary>
        /// 昨日收盘价
        /// </summary>
        public decimal PreClosingPrice { get; set; }

        /// <summary>
        /// 涨幅
        /// </summary>
        public decimal Gain { get; set; }

        /// <summary>
        /// 预测涨幅
        /// </summary>
        public PredictionType Prediction { get; set; }

        /// <summary>
        /// 仓位比例
        /// </summary>
        public decimal PositionRate { get; set; }

        /// <summary>
        /// 仓位类型
        /// </summary>
        public PositionType PositionType { get; set; }

        /// <summary>
        /// 预测偏离度
        /// </summary>
        public int? PredictionDeviationValue { get; set; }

        /// <summary>
        /// 预测评分
        /// </summary>
        public decimal? MockScore { get; set; }

        /// <summary>
        /// 持仓金额
        /// </summary>
        public decimal PositionAmount { get; set; }

        /// <summary>
        /// 盈亏金额
        /// </summary>
        public decimal EarningsAmount { get; set; }
    }
}
