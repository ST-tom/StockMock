using log4net;
using log4net.Repository;
using System.Collections.Concurrent;

namespace StockMock.Infrastructure.Utils
{
    /// <summary>
    /// Log4Net日志类
    /// </summary>
    public class Log4NetUtil
    {
        private static string repositoryName = "ConsoleRepository";
        private static ILoggerRepository repository = null;
        private static ConcurrentDictionary<Type, ILog> dic_Loggers = null;  //记录器字典
        private static string strConfPath = "log4net.config";  //配置文件名称
        private static object objlock = new object();  //初始化用的锁

        #region 初始化日志设置
        /// <summary>
        /// 初始化日志设置
        /// </summary>
        /// <returns></returns>
        public static bool InitLog4net()
        {
            if (repository != null && dic_Loggers != null)
            {
                return true;
            }

            lock (objlock)
            {
                FileInfo ee = new FileInfo(strConfPath);
                repository = LogManager.CreateRepository(repositoryName);
                dic_Loggers = new ConcurrentDictionary<Type, ILog>();
                log4net.Config.XmlConfigurator.ConfigureAndWatch(repository, new FileInfo(strConfPath));
                return true;
            }
        }
        #endregion

        #region 获取记录器
        /// <summary>
        /// 获取记录器
        /// </summary>
        /// <param name="source">soruce</param>
        /// <returns></returns>
        private static ILog GetLogger(Type source)
        {
            if (dic_Loggers.ContainsKey(source))
            {
                return dic_Loggers[source];
            }
            else
            {
                ILog logger = LogManager.GetLogger(repository.Name, source);
                dic_Loggers.TryAdd(source, logger);
                return logger;
            }
        }
        #endregion

        #region Debug
        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="source">源类类型</param>
        /// <param name="strMsg">日志信息</param>
        public static void Debug(Type source, string strMsg)
        {
            //存储库检查
            if (InitLog4net())
            {
                ILog logger = GetLogger(source);
                if (logger.IsDebugEnabled)
                {
                    logger.Debug(strMsg);
                }
            }
        }

        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="source">源类</param>
        /// <param name="strMsg">日志信息</param>
        public static void Debug(object source, string strMsg)
        {
            Debug(source.GetType(), strMsg);
        }
        #endregion

        #region Info
        /// <summary>
        /// 日常信息
        /// </summary>
        /// <param name="source">源类类型</param>
        /// <param name="strMsg">日志信息</param>
        public static void Info(Type source, string strMsg)
        {
            //存储库检查
            if (InitLog4net())
            {
                ILog logger = GetLogger(source);
                if (logger.IsInfoEnabled)
                {
                    logger.Info(strMsg);
                }
            }
        }

        /// <summary>
        /// 日常信息
        /// </summary>
        /// <param name="source">源类</param>
        /// <param name="strMsg">日志信息</param>
        public static void Info(object source, string strMsg)
        {
            Info(source.GetType(), strMsg);
        }
        #endregion

        #region Warn
        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="source">源类类型</param>
        /// <param name="strMsg">日志信息</param>
        public static void Warn(Type source, string strMsg)
        {
            //存储库检查
            if (InitLog4net())
            {
                ILog logger = GetLogger(source);
                if (logger.IsWarnEnabled)
                {
                    logger.Warn(strMsg);
                }
            }
        }

        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="source">源类</param>
        /// <param name="strMsg">日志信息</param>
        public static void Warn(object source, string strMsg)
        {
            Warn(source.GetType(), strMsg);
        }
        #endregion

        #region Error
        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="source">源类类型</param>
        /// <param name="strMsg">日志信息</param>
        public static void Error(Type source, string strMsg)
        {
            //存储库检查
            if (InitLog4net())
            {
                ILog logger = GetLogger(source);
                if (logger.IsErrorEnabled)
                {
                    logger.Error(strMsg);
                }
            }
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="source">源类</param>
        /// <param name="strMsg">日志信息</param>
        public static void Error(object source, string strMsg)
        {
            Error(source.GetType(), strMsg);
        }
        #endregion

        #region Fatal
        /// <summary>
        /// 严重错误信息
        /// </summary>
        /// <param name="source">源类类型</param>
        /// <param name="strMsg">日志信息</param>
        public static void Fatal(Type source, string strMsg)
        {
            //存储库检查
            if (InitLog4net())
            {
                ILog logger = GetLogger(source);
                if (logger.IsFatalEnabled)
                {
                    logger.Fatal(strMsg);
                }
            }
        }

        /// <summary>
        /// 严重错误信息
        /// </summary>
        /// <param name="source">源类</param>
        /// <param name="strMsg">日志信息</param>
        public static void Fatal(object source, string strMsg)
        {
            Fatal(source.GetType(), strMsg);
        }
        #endregion
    }
}
