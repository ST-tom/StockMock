using StockMock.Core;
using StockMock.Core.Accounts;

namespace StockMock.Core.Stocks
{
    public class AccountStock : BaseEntity
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long AccountId { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 
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

        public Stock Stock { get; set; }

        public Account Account { get; set; }
    }
}
