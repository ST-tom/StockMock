namespace StockMock.Core.Logs
{
    /// <summary>
    /// 通用基础日志实体
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 日志ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 日志时间戳
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 日志级别
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 日志消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 日志消息模板
        /// </summary>
        public string MessageTemplate { get; set; }

        /// <summary>
        /// 日志属性（JSON格式）
        /// </summary>
        public string Properties { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 操作名称
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// 机器名称
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// 线程ID
        /// </summary>
        public int ThreadId { get; set; }
    }
}