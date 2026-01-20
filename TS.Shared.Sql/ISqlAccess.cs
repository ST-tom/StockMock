using Microsoft.Data.SqlClient;
using System.Data;

namespace TS.Shared.Sql
{
    public interface ISqlAccess
    {
        #region 测试连接

        /// <summary>
        /// 测试连接
        /// </summary>
        bool TestConn();

        /// <summary>
        /// 测试连接
        /// </summary>
        Task<bool> TestConnAsync();

        #endregion

        #region 判断表是否存在

        /// <summary>
        /// 判断表是否存在
        /// </summary>
        bool IsTableExist(string tableName);

        /// <summary>
        /// 判断表是否存在
        /// </summary>
        Task<bool> IsTableExistAsync(string tableName);

        #endregion

        #region 查询单条数据

        /// <summary>
        /// 查询单条数据
        /// </summary>
        T QueryObj<T>(string sql, SqlParameter[]? parameters = null, int timeout = 30);

        /// <summary>
        /// 查询单条数据
        /// </summary>
        Task<T> QueryObjAsync<T>(string sql, SqlParameter[]? parameters = null, int timeout = 30, CancellationToken cancellationToken = default);

        #endregion

        #region 批量查询

        /// <summary>
        /// 批量查询多条数据
        /// </summary>
        List<T> QueryLargeList<T>(string sql, SqlParameter[]? parameters = null, int timeout = 30);

        /// <summary>
        /// 批量查询多条数据
        /// </summary>
        Task<List<T>> QueryLargeListAsync<T>(string sql, SqlParameter[]? parameters = null, int timeout = 30, CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量查询
        /// </summary>
        List<T> QueryList<T>(string sql, SqlParameter[]? parameters = null, int timeout = 30);

        /// <summary>
        /// 批量查询
        /// </summary>
        Task<List<T>> QueryListAsync<T>(string sql, SqlParameter[]? parameters = null, int timeout = 30, CancellationToken cancellationToken = default);

        #endregion

        #region 查询数据表

        /// <summary>
        /// 查询数据表
        /// </summary>
        DataTable QueryDataTable(string sql, SqlParameter[]? parameters = null, int timeout = 30);

        /// <summary>
        /// 查询数据表
        /// </summary>
        Task<DataTable> QueryDataTableAsync(string sql, SqlParameter[]? parameters = null, int timeout = 30, CancellationToken cancellationToken = default);

        #endregion

        #region 执行

        /// <summary>
        /// 执行
        /// </summary>
        int Execute(string sql, SqlParameter[]? parameters = null, int timeout = 3);

        /// <summary>
        /// 执行
        /// </summary>
        Task<int> ExecuteAsync(string sql, SqlParameter[]? parameters = null, int timeout = 30, CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行事务
        /// </summary>
        int ExecuteTran(string sql, SqlParameter[]? parameters = null, int timeout = 30);

        /// <summary>
        /// 执行事务
        /// </summary>
        Task<int> ExecuteTranAsync(string sql, SqlParameter[]? parameters = null, int timeout = 30, CancellationToken cancellationToken = default);

        #endregion

        #region 批量插入

        /// <summary>
        /// 批量插入
        /// </summary>
        void BulkCopyInsert(string tableName, DataTable dt, Dictionary<string, string>? mapColumnDic, int batchSize = 5000, int timeout = 300, bool isNotify = false);

        /// <summary>
        /// 批量插入
        /// </summary>
        Task BulkCopyInsertAsync(string tableName, DataTable dt, Dictionary<string, string>? mapColumnDic, CancellationToken cancellationToken = default, int batchSize = 5000, int timeout = 300, bool isNotify = false);

        /// <summary>
        /// 批量插入
        /// </summary>
        void BulkCopyInsert<T>(string tableName, IEnumerable<T> items, Dictionary<string, string>? mapColumnDic, int batchSize = 5000, int timeout = 300, bool isNotify = false);

        /// <summary>
        /// 批量插入
        /// </summary>
        Task BulkCopyInsertAsync<T>(string tableName, IEnumerable<T> items, Dictionary<string, string>? mapColumnDic, CancellationToken cancellationToken = default, int batchSize = 5000, int timeout = 300, bool isNotify = false);

        #endregion
    }
}
