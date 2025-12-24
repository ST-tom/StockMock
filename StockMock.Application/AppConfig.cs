using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace StockMock.Application
{
    public class ApplicationConfig
    {
        #region 模拟数据单股仓位最大金额/元

        /// <summary>
        /// 模拟数据单股仓位最大金额/元
        /// </summary>
        public decimal mock_position_max_amount = 100000;

        #endregion

        #region 初始化配置文件

        public void Init(IHostApplicationBuilder builder)
        {
            var configuration = builder.Configuration;
            mock_position_max_amount = configuration.GetSection("Stock").GetValue<int>("MockPositionMaxAmount", 100000);
        }

        #endregion
    }
}
