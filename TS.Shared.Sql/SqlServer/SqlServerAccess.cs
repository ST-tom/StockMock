using Microsoft.Data.SqlClient;
using System.Data;
using TS.Shared.Extension;

namespace TS.Shared.Sql.SqlServer
{
    /// <summary>
    /// SqlServer数据库访问
    /// </summary>
    /// <param name="connStr"></param>
    /// <param name="SqlRowsCopiedNotify"></param>
    public class SqlServerAccess(string connStr, Action<object, SqlRowsCopiedEventArgs>? SqlRowsCopiedNotify = default) : ISqlAccess
    {
        protected readonly string connStr = connStr;

        #region 批次处理通知事件

        /// <summary>
        /// 批次处理通知事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected Action<object, SqlRowsCopiedEventArgs>? SqlRowsCopiedNotify = SqlRowsCopiedNotify;

        #endregion

        #region 获取SqlConnection

        /// <summary>
        /// 获取SqlConnection
        /// </summary>
        private async Task<SqlConnection> GetSqlConnAsync(CancellationToken cancellationToken = default, bool isOpen = true)
        {
            var connection = new SqlConnection(this.connStr);

            if (isOpen)
                await connection.OpenAsync(cancellationToken);

            return connection;
        }

        /// <summary>
        /// 获取SqlConnection
        /// </summary>
        private SqlConnection GetSqlConn(bool isOpen = true)
        {
            var connection = new SqlConnection(this.connStr);

            if (isOpen)
                connection.Open();

            return connection;
        }

        #endregion

        #region 测试连接

        /// <summary>
        /// 测试连接
        /// </summary>
        public bool TestConn()
        {
            try
            {
                using SqlConnection sqlConnection = GetSqlConn();
                return sqlConnection.State == ConnectionState.Open;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 测试连接
        /// </summary>
        public async Task<bool> TestConnAsync()
        {
            try
            {
                using SqlConnection sqlConnection = await GetSqlConnAsync().ConfigureAwait(false);
                return sqlConnection.State == ConnectionState.Open;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 判断表是否存在

        /// <summary>
        /// 判断表是否存在
        /// </summary>
        public bool IsTableExist(string tableName)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

                using SqlConnection sqlConnection = GetSqlConn();
                string sql = $"SELECT COUNT(*) FROM sysobjects WHERE xtype='U' and [name]='{tableName}';";

                return QueryObj<int>(sql) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 判断表是否存在
        /// </summary>
        public async Task<bool> IsTableExistAsync(string tableName)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

                using SqlConnection sqlConnection = await GetSqlConnAsync().ConfigureAwait(false);
                string sql = $"SELECT COUNT(*) FROM sysobjects WHERE xtype='U' and [name]='{tableName}';";

                return await QueryObjAsync<int>(sql).ConfigureAwait(false) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region 查询单条数据

        /// <summary>
        /// 查询单条数据
        /// </summary>
        public T QueryObj<T>(string sql, SqlParameter[]? parameters = null, int timeout = 30)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(sql);

            using SqlConnection sqlConnection = GetSqlConn();
            using SqlCommand cmd = new();
            cmd.Connection = sqlConnection;
            cmd.CommandText = sql;
            cmd.CommandTimeout = timeout <= 30 ? 30 : timeout;
            cmd.CommandType = CommandType.Text;
            if (parameters.HasData())
                cmd.Parameters.AddRange(parameters);

            if (sqlConnection.State != ConnectionState.Open)
                sqlConnection.Open();

            using SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow | CommandBehavior.CloseConnection);

            return reader.Read() ? reader.ToObject<T>() : default!;
        }

        /// <summary>
        /// 查询单条数据
        /// </summary>
        public async Task<T> QueryObjAsync<T>(string sql, SqlParameter[]? parameters = null, int timeout = 30, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(sql);

            using SqlConnection sqlConnection = await GetSqlConnAsync(cancellationToken).ConfigureAwait(false);
            using SqlCommand cmd = new();
            cmd.Connection = sqlConnection;
            cmd.CommandText = sql;
            cmd.CommandTimeout = timeout <= 30 ? 30 : timeout;
            cmd.CommandType = CommandType.Text;
            if (parameters.HasData())
                cmd.Parameters.AddRange(parameters);

            if (sqlConnection.State != ConnectionState.Open)
                await sqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

            using SqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow | CommandBehavior.CloseConnection, cancellationToken).ConfigureAwait(false);

            return await reader.ReadAsync(cancellationToken).ConfigureAwait(false) ? reader.ToObject<T>() : default!;
        }

        #endregion

        #region 批量查询

        /// <summary>
        /// 批量查询多条数据
        /// </summary>
        public List<T> QueryLargeList<T>(string sql, SqlParameter[]? parameters = null, int timeout = 30)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(sql);

            using SqlConnection sqlConnection = GetSqlConn();
            using SqlCommand cmd = new();
            cmd.Connection = sqlConnection;
            cmd.CommandText = sql;
            cmd.CommandTimeout = timeout <= 30 ? 30 : timeout;
            cmd.CommandType = CommandType.Text;
            if (parameters.HasData())
                cmd.Parameters.AddRange(parameters);

            if (sqlConnection.State != ConnectionState.Open)
                sqlConnection.Open();

            using SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            List<T> list = [];
            while (reader.Read())
            {
                list.Add(reader.ToObject<T>());
            }
            return list;
        }

        /// <summary>
        /// 批量查询多条数据
        /// </summary>
        public async Task<List<T>> QueryLargeListAsync<T>(string sql, SqlParameter[]? parameters = null, int timeout = 30, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(sql);

            using SqlConnection sqlConnection = await GetSqlConnAsync(cancellationToken).ConfigureAwait(false);
            using SqlCommand cmd = new();
            cmd.Connection = sqlConnection;
            cmd.CommandText = sql;
            cmd.CommandTimeout = timeout <= 30 ? 30 : timeout;
            cmd.CommandType = CommandType.Text;
            if (parameters.HasData())
                cmd.Parameters.AddRange(parameters);

            if (sqlConnection.State != ConnectionState.Open)
                await sqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

            using SqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken).ConfigureAwait(false);

            List<T> list = [];
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                list.Add(reader.ToObject<T>());
            }

            return list;
        }

        /// <summary>
        /// 批量查询
        /// </summary>
        public List<T> QueryList<T>(string sql, SqlParameter[]? parameters = null, int timeout = 30)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(sql);

            using SqlConnection sqlConnection = GetSqlConn();
            using SqlCommand cmd = new();
            cmd.Connection = sqlConnection;
            cmd.CommandText = sql;
            cmd.CommandTimeout = timeout <= 30 ? 30 : timeout;
            cmd.CommandType = CommandType.Text;
            if (parameters.HasData())
                cmd.Parameters.AddRange(parameters);

            if (sqlConnection.State != ConnectionState.Open)
                sqlConnection.Open();

            using SqlDataAdapter adapter = new(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt.ToList<T>();
        }

        /// <summary>
        /// 批量查询
        /// </summary>
        public async Task<List<T>> QueryListAsync<T>(string sql, SqlParameter[]? parameters = null, int timeout = 30, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(sql);

            using SqlConnection sqlConnection = await GetSqlConnAsync(cancellationToken).ConfigureAwait(false);
            using SqlCommand cmd = new();
            cmd.Connection = sqlConnection;
            cmd.CommandText = sql;
            cmd.CommandTimeout = timeout <= 30 ? 30 : timeout;
            cmd.CommandType = CommandType.Text;
            if (parameters.HasData())
                cmd.Parameters.AddRange(parameters);

            if (sqlConnection.State != ConnectionState.Open)
                await sqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

            using SqlDataAdapter adapter = new(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt.ToList<T>();
        }

        #endregion

        #region 查询数据表

        /// <summary>
        /// 查询数据表
        /// </summary>
        public DataTable QueryDataTable(string sql, SqlParameter[]? parameters = null, int timeout = 30)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(sql);

            using SqlConnection sqlConnection = GetSqlConn();
            using SqlCommand cmd = new();
            cmd.Connection = sqlConnection;
            cmd.CommandText = sql;
            cmd.CommandTimeout = timeout <= 30 ? 30 : timeout;
            cmd.CommandType = CommandType.Text;
            if (parameters.HasData())
                cmd.Parameters.AddRange(parameters);

            using SqlDataAdapter adapter = new(cmd);
            DataTable table = new();
            adapter.Fill(table);

            return table;
        }

        /// <summary>
        /// 查询数据表
        /// </summary>
        public async Task<DataTable> QueryDataTableAsync(string sql, SqlParameter[]? parameters = null, int timeout = 30, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(sql);

            using SqlConnection sqlConnection = await GetSqlConnAsync(cancellationToken).ConfigureAwait(false);
            using SqlCommand cmd = new();
            cmd.Connection = sqlConnection;
            cmd.CommandText = sql;
            cmd.CommandTimeout = timeout <= 30 ? 30 : timeout;
            cmd.CommandType = CommandType.Text;
            if (parameters.HasData())
                cmd.Parameters.AddRange(parameters);

            using SqlDataAdapter adapter = new(cmd);
            DataTable table = new();
            adapter.Fill(table);

            return table;
        }

        #endregion

        #region 执行

        /// <summary>
        /// 执行
        /// </summary>
        public int Execute(string sql, SqlParameter[]? parameters = null, int timeout = 30)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(sql);
            using SqlConnection sqlConnection = GetSqlConn();
            using SqlCommand cmd = new();
            cmd.Connection = sqlConnection;
            cmd.CommandText = sql;
            cmd.CommandTimeout = timeout <= 30 ? 30 : timeout;
            cmd.CommandType = CommandType.Text;

            if (parameters.HasData())
                cmd.Parameters.AddRange(parameters);

            if (sqlConnection.State != ConnectionState.Open)
                sqlConnection.Open();

            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 执行
        /// </summary>
        public async Task<int> ExecuteAsync(string sql, SqlParameter[]? parameters = null, int timeout = 30, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(sql);
            using SqlConnection sqlConnection = await GetSqlConnAsync(cancellationToken).ConfigureAwait(false);
            using SqlCommand cmd = new();
            cmd.Connection = sqlConnection;
            cmd.CommandText = sql;
            cmd.CommandTimeout = timeout <= 30 ? 30 : timeout;
            cmd.CommandType = CommandType.Text;

            if (parameters.HasData())
                cmd.Parameters.AddRange(parameters);

            if (sqlConnection.State != ConnectionState.Open)
                await sqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

            return await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 执行事务
        /// </summary
        public int ExecuteTran(string sql, SqlParameter[]? parameters = null, int timeout = 30)
        {
            SqlTransaction? sqlTran = null;
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(sql);
                using SqlConnection sqlConnection = GetSqlConn();

                using SqlCommand cmd = new();
                cmd.Connection = sqlConnection;
                cmd.CommandText = sql;
                cmd.CommandTimeout = timeout <= 30 ? 30 : timeout;
                cmd.CommandType = CommandType.Text;
                if (parameters.HasData())
                    cmd.Parameters.AddRange(parameters);

                if (sqlConnection.State != ConnectionState.Open)
                    sqlConnection.Open();

                sqlTran = sqlConnection.BeginTransaction();
                cmd.Transaction = sqlTran;

                int count = cmd.ExecuteNonQuery();
                sqlTran.Commit();

                return count;
            }
            catch (Exception ex)
            {
                try
                {
                    sqlTran?.Rollback();
                }
                catch (Exception rollbackEx)
                {
                    throw new AggregateException("提交失败且回滚异常", ex, rollbackEx);
                }
                throw;
            }
            finally
            {
                sqlTran?.Dispose();
            }
        }

        /// <summary>
        /// 执行事务
        /// </summary
        public async Task<int> ExecuteTranAsync(string sql, SqlParameter[]? parameters = null, int timeout = 30, CancellationToken cancellationToken = default)
        {
            SqlTransaction? sqlTran = null;
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(sql);
                using SqlConnection sqlConnection = await GetSqlConnAsync(cancellationToken).ConfigureAwait(false);

                using SqlCommand cmd = new();
                cmd.Connection = sqlConnection;
                cmd.CommandText = sql;
                cmd.CommandTimeout = timeout <= 30 ? 30 : timeout;
                cmd.CommandType = CommandType.Text;
                if (parameters.HasData())
                    cmd.Parameters.AddRange(parameters);

                if (sqlConnection.State != ConnectionState.Open)
                    await sqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

                sqlTran = sqlConnection.BeginTransaction();
                cmd.Transaction = sqlTran;

                int count = await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                await sqlTran.CommitAsync(CancellationToken.None);

                return count;
            }
            catch (Exception ex)
            {
                try
                {
                    if (sqlTran != null)
                        await sqlTran.RollbackAsync(CancellationToken.None);
                }
                catch (Exception rollbackEx)
                {
                    throw new AggregateException("提交失败且回滚异常", ex, rollbackEx);
                }
                throw;
            }
            finally
            {
                if (sqlTran != null)
                    await sqlTran.DisposeAsync();
            }
        }

        #endregion

        #region 批量插入

        /// <summary>
        /// 批量插入
        /// </summary
        public void BulkCopyInsert(string tableName, DataTable dt, Dictionary<string, string>? mapColumnDic, int batchSize = 5000, int timeout = 300, bool isNotify = false)
        {
            SqlTransaction? sqlTran = null;
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
                if (dt == null || dt.Rows.Count == 0)
                    return;

                using SqlConnection sqlConn = GetSqlConn();
                sqlTran = sqlConn.BeginTransaction();

                using var bulkCopy = new SqlBulkCopy(sqlConn, SqlBulkCopyOptions.UseInternalTransaction, sqlTran)
                {
                    DestinationTableName = tableName.Trim(), // 目标表名
                    BatchSize = batchSize,
                    BulkCopyTimeout = timeout <= 300 ? 300 : timeout,
                };

                if (mapColumnDic.HasData())
                {
                    mapColumnDic!.ForEach(item => bulkCopy.ColumnMappings.Add(item.Key, item.Value));
                }
                else
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        var column = dt.Columns[i];
                        bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }
                }

                //是否按批次通知
                if (isNotify && SqlRowsCopiedNotify != null)
                {
                    bulkCopy.NotifyAfter = bulkCopy.BatchSize;
                    bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(SqlRowsCopiedNotify);
                }

                bulkCopy.WriteToServer(dt);
                sqlTran.Commit();
            }
            catch (Exception ex)
            {
                try
                {
                    sqlTran?.Rollback();
                }
                catch (Exception rollbackEx)
                {
                    throw new AggregateException("提交失败且回滚异常", ex, rollbackEx);
                }

                throw;
            }
            finally
            {
                sqlTran?.Dispose();
            }
        }

        /// <summary>
        /// 批量插入
        /// </summary
        public async Task BulkCopyInsertAsync(string tableName, DataTable dt, Dictionary<string, string>? mapColumnDic, CancellationToken cancellationToken = default, int batchSize = 5000, int timeout = 300, bool isNotify = false)
        {
            SqlTransaction? sqlTran = null;
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
                if (dt == null || dt.Rows.Count == 0)
                    return;

                using SqlConnection sqlConn = await GetSqlConnAsync(cancellationToken).ConfigureAwait(false);
                sqlTran = sqlConn.BeginTransaction();

                using var bulkCopy = new SqlBulkCopy(sqlConn, SqlBulkCopyOptions.UseInternalTransaction, sqlTran)
                {
                    DestinationTableName = tableName.Trim(), // 目标表名
                    BatchSize = batchSize,
                    BulkCopyTimeout = timeout <= 300 ? 300 : timeout,
                };

                if (mapColumnDic.HasData())
                {
                    mapColumnDic!.ForEach(item => bulkCopy.ColumnMappings.Add(item.Key, item.Value));
                }
                else
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        var column = dt.Columns[i];
                        bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }
                }

                //是否按批次通知
                if (isNotify && SqlRowsCopiedNotify != null)
                {
                    bulkCopy.NotifyAfter = bulkCopy.BatchSize;
                    bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(SqlRowsCopiedNotify);
                }

                await bulkCopy.WriteToServerAsync(dt, cancellationToken);
                await sqlTran.CommitAsync();
            }
            catch (Exception ex)
            {
                try
                {
                    if (sqlTran != null)
                        await sqlTran.RollbackAsync();
                }
                catch (Exception rollbackEx)
                {
                    throw new AggregateException("提交失败且回滚异常", ex, rollbackEx);
                }
                throw;
            }
            finally
            {
                if (sqlTran != null)
                    await sqlTran.DisposeAsync();
            }
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        public void BulkCopyInsert<T>(string tableName, IEnumerable<T> items, Dictionary<string, string>? mapColumnDic, int batchSize = 5000, int timeout = 300, bool isNotify = false)
        {
            using SqlConnection sqlConn = GetSqlConn();
            var dt = items.ToDataTable(mapColumnDic);

            BulkCopyInsert(tableName, dt, null, batchSize, timeout, isNotify);
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        public async Task BulkCopyInsertAsync<T>(string tableName, IEnumerable<T> items, Dictionary<string, string>? mapColumnDic, CancellationToken cancellationToken = default, int batchSize = 5000, int timeout = 300, bool isNotify = false)
        {
            using SqlConnection sqlConn = await GetSqlConnAsync(cancellationToken).ConfigureAwait(false);
            var dt = items.ToDataTable(mapColumnDic);

            await BulkCopyInsertAsync(tableName, dt, null, cancellationToken, batchSize, timeout, isNotify);
        }

        #endregion
    }
}
