using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;
using TS.Shared.Sql.SqlServer;
using TS.Shared.Managers;
using TS.Shared.Extension;

namespace TS.Shared.Sql.SqlServer
{
    public static class SqlDataReaderExtension
    {
        public static T ToObject<T>(this SqlDataReader reader)
        {
            Type targetType = typeof(T);

            // 处理值类型/字符串等简单类型（单行单列场景）
            if (targetType.IsValueType || targetType == typeof(string))
            {
                // 读取第一列的值，为空则返回默认值
                object value = reader.IsDBNull(0) ? default! : reader.GetValue(0);
                return (T)Convert.ChangeType(value, targetType);
            }

            T instance = Activator.CreateInstance<T>();
            var (propertyDic, fieldDic) = EntityPropertyAndFieldManager.Instance.GetPropertyAndFieldForReadAndWrite<T>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.IsDBNull(i))
                    continue;

                var name = reader.GetName(i);
                var value = reader.GetValue(i);

                if (propertyDic.TryGetValue(name, out PropertyInfo? propertyInfo))
                    propertyInfo.SetValue(instance, Convert.ChangeType(value, propertyInfo.PropertyType));
                else if(fieldDic.TryGetValue(name, out FieldInfo? filedInfo))
                    filedInfo.SetValue(instance, Convert.ChangeType(value, filedInfo.FieldType));
            }

            return instance;
        }

        public static List<T> ToList<T>(this DataTable dataTable)
        {
            return dataTable.Rows.Cast<DataRow>().Select(row => row.ToObject<T>()).ToList();
        }

        private static T ToObject<T>(this DataRow row)
        {
            Type targetType = typeof(T);

            // 处理值类型/字符串等简单类型（单行单列场景）
            if (targetType.IsSimpleType())
            {
                // 读取第一列的值，为空则返回默认值
                object value = row.IsNull(0) ? default! : row[0];
                return (T)Convert.ChangeType(value, targetType);
            }

            T instance = Activator.CreateInstance<T>();
            var (propertyDic, fieldDic) = EntityPropertyAndFieldManager.Instance.GetPropertyAndFieldForReadAndWrite<T>();

            foreach (DataColumn column in row.Table.Columns)
            {
                if (row.IsNull(column))
                    continue;

                if (propertyDic.TryGetValue(column.ColumnName, out PropertyInfo? propertyInfo))
                    propertyInfo.SetValue(instance, Convert.ChangeType(row[column], propertyInfo.PropertyType));
                else if (fieldDic.TryGetValue(column.ColumnName, out FieldInfo? fieldInfo))
                    fieldInfo.SetValue(instance, Convert.ChangeType(row[column], fieldInfo.FieldType));
            }

            return instance;
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> items, Dictionary<string, string>? mapColumnDic)
        {
            var propertyDic = EntityPropertyAndFieldManager.Instance.GetReadProperty<T>();

            var dataTable = new DataTable();

            foreach (var node in propertyDic)
            {
                var property = node.Value;
                if (mapColumnDic?.TryGetValue(property.Name, out string? columnName) == true)
                {
                    var columnType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    dataTable.Columns.Add(columnName, columnType);
                }
            }

            foreach (var item in items)
            {
                if (item == null)
                    continue;

                var row = dataTable.NewRow();
                foreach (var node in propertyDic)
                {
                    var property = node.Value;
                    var value = property.GetValue(item) ?? DBNull.Value;
                    row[property.Name] = value;
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}
