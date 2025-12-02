using StockMock.Domain.Common;
using StockMock.Domain.Entities.Stocks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockMock.Domain.Entities.Mocks
{
    public class Mock : BaseEntity
    {
        /// <summary>
        /// 股票编号
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
        /// 可补仓金额
        /// </summary>
        public decimal ToppingUpAmount { get; set; }

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
        [NotMapped]
        public decimal TotalAmount => BaseAmount + EarningsAmount;

        /// <summary>
        /// 收益率
        /// </summary>
        public decimal EarningsRate { get; set; }

        public List<MockDate> MockDates { get; } = new List<MockDate>();

        public Stock Stock { get; set; }
    }

    public enum MockStatus
    {
        [Description("新建")]
        created,
        [Description("作废")]
        canceled,
        [Description("模拟中")]
        mocking,
        [Description("模拟结束")]
        finished
    }
}
