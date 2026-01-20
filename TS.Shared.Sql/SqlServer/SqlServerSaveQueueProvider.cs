using TS.Shared.SaveQueue;

namespace TS.Shared.Sql.SqlServer
{
    public class SqlServerSaveQueueProvider<T>(
        string tableName,
        ISqlAccess sqlAccess)
        : ISaveQueueProvider<T>
    {
        private readonly string tableName = tableName;

        private readonly ISqlAccess sqlAccess = sqlAccess;

        public async Task<int> SaveBatchAsync(IEnumerable<T> items, CancellationToken cancellationToken = default)
        {
            if (items == null || !items.Any())
                return 0;

            await this.sqlAccess.BulkCopyInsertAsync(this.tableName, items, null, cancellationToken);      
            return items.Count();
        }
    }
}
