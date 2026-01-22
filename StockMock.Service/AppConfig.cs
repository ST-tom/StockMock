using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace StockMock.Service
{
    public class AppConfig
    {
        #region 模拟数据单股仓位最大金额/元

        /// <summary>
        /// 模拟数据单股仓位最大金额/元
        /// </summary>
        public static decimal mock_position_max_amount = 100000;

        #endregion

        #region 初始化配置文件

        public static void Init(IHostApplicationBuilder builder)
        {
            var configuration = builder.Configuration;
        }

        #endregion
    }
}
