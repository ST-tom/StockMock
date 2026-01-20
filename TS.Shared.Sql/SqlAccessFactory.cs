
using TS.Shared.Sql.SqlServer;

namespace TS.Shared.Sql
{
    public class SqlAccessFactory
    {
        public static ISqlAccess CreateSqlAccess(EnumDbType dbType, string connStr)
        {
            return dbType switch
            {
                EnumDbType.SqlServer => new SqlServerAccess(connStr),
                _ => throw new ArgumentException("不支持的数据库类型"),
            };
        }
    }
}
