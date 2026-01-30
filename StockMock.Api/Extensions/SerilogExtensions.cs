namespace StockMock.Api.Extensions
{
    public static class SerilogExtensions
    {
        /// <summary>
        /// 向日志上下文添加用户信息
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="userId">用户ID</param>
        /// <param name="userName">用户名</param>
        /// <returns>日志记录器</returns>
        public static Serilog.ILogger WithUser(this Serilog.ILogger logger, int? userId, string? userName)
        {
            if (userId.HasValue)
                logger = logger.ForContext("UserId", userId.Value);
            
            if (!string.IsNullOrEmpty(userName))
                logger = logger.ForContext("UserName", userName);
            
            return logger;
        }

        /// <summary>
        /// 向日志上下文添加操作信息
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="action">操作名称</param>
        /// <param name="module">模块名称</param>
        /// <returns>日志记录器</returns>
        public static Serilog.ILogger WithOperation(this Serilog.ILogger logger, string? action, string? module)
        {
            if (!string.IsNullOrEmpty(action))
                logger = logger.ForContext("Action", action);
            
            if (!string.IsNullOrEmpty(module))
                logger = logger.ForContext("Module", module);
            
            return logger;
        }

        /// <summary>
        /// 向日志上下文添加自定义字段
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="properties">自定义属性字典</param>
        /// <returns>日志记录器</returns>
        public static Serilog.ILogger WithProperties(this Serilog.ILogger logger, System.Collections.Generic.Dictionary<string, object> properties)
        {
            foreach (var property in properties)
            {
                logger = logger.ForContext(property.Key, property.Value);
            }
            
            return logger;
        }
    }
}