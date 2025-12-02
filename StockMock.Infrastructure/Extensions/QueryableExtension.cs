using System.Linq.Expressions;
using System.Reflection;

namespace StockMock.Infrastructure.Extensions
{
    public static class QueryableExtension
    {
        public static IQueryable<T> OrderyByField<T>(this IQueryable<T> source, string field, bool isDesc = false)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (string.IsNullOrWhiteSpace(field))
                return source;

            Type type = typeof(T);
            PropertyInfo? property = type.GetProperty(field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (property == null)
                throw new ArgumentException($"实体 {type.Name} 中不存在字段 {field}");

            // 构建表达式树：x => x.Property
            ParameterExpression parameter = Expression.Parameter(type, "x");
            Expression propertySelector = Expression.Property(parameter, property);
            var orderby = Expression.Lambda<Func<T, object>>(propertySelector, parameter);
            if (isDesc)
                source = source.OrderByDescending(orderby);
            else
                source = source.OrderBy(orderby);

            return source;
        }
    }
}
