using System.Collections.Concurrent;
using System.Reflection;

namespace TS.Shared.Managers
{
    public class EntityPropertyAndFieldManager
    {
        private readonly ConcurrentDictionary<Type, (Dictionary<string, PropertyInfo> readAndWritePropertyDic, Dictionary<string, PropertyInfo> readPropertyDic, Dictionary<string, FieldInfo> fieldDic)> dic = new();

        private static readonly Lazy<EntityPropertyAndFieldManager> _lazyInstance = new(() => new EntityPropertyAndFieldManager(), LazyThreadSafetyMode.ExecutionAndPublication);

        public static EntityPropertyAndFieldManager Instance => _lazyInstance.Value;

        private static (Dictionary<string, PropertyInfo> readAndWritePropertyDic, Dictionary<string, PropertyInfo> readPropertyDic, Dictionary<string, FieldInfo> fieldDic) TypeToDic(Type type)
        {
            var propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            var readAndWritePropertyDic = propertys.Where(e => e.CanWrite && e.CanRead).ToDictionary(e => e.Name, e => e);
            var readPropertyDic = propertys.Where(e => e.CanRead).ToDictionary(e => e.Name, e => e);
            var fieldDic = fields.ToDictionary(e => e.Name, e => e);

            return (readAndWritePropertyDic, readPropertyDic, fieldDic);
        }

        public (Dictionary<string, PropertyInfo> propertyDic, Dictionary<string, FieldInfo> fieldDic) GetPropertyAndFieldForReadAndWrite<T>(bool isSave = true)
        {
            var type = typeof(T);

            if (this.dic.TryGetValue(type, out var dic))
                return (dic.readAndWritePropertyDic, dic.fieldDic);

            var infos = TypeToDic(type);

            if (isSave)
                this.dic.TryAdd(type, infos);

            return (infos.readAndWritePropertyDic, infos.fieldDic);
        }

        public Dictionary<string, PropertyInfo> GetReadProperty<T>(bool isSave = true)
        {
            var type = typeof(T);

            if (this.dic.TryGetValue(type, out var dic))
                return dic.readPropertyDic;

            var propertyDic = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToDictionary(item => item.Name, item => item);
            var fieldDic = type.GetFields(BindingFlags.Public | BindingFlags.Instance).ToDictionary(item => item.Name, item => item);

            var infos = TypeToDic(type);
            if (isSave)
                this.dic.TryAdd(type, infos);

            return infos.readPropertyDic;
        }

        //public string TryGetString<T>(T obj, string key)
        //{
        //    var type = typeof(T);

        //    var dic = GetPropertyAndFieldForReadAndWrite<T>();

        //    if(dic.propertyDic.TryGetValue(key, out var propertyInfo))
        //        return propertyInfo.GetValue(obj)?.ToString() ?? string.Empty;

        //    if(dic.fieldDic.TryGetValue(key, out var fieldInfo))
        //        return fieldInfo.GetValue(obj)?.ToString() ?? string.Empty;

        //    return string.Empty;
        //}

        //public object? TryGetObject<T>(T obj, string key)
        //{
        //    var type = typeof(T);

        //    var dic = GetPropertyAndFieldForReadAndWrite<T>();

        //    if (dic.propertyDic.TryGetValue(key, out var propertyInfo))
        //        return propertyInfo.GetValue(obj);

        //    if (dic.fieldDic.TryGetValue(key, out var fieldInfo))
        //        return fieldInfo.GetValue(obj);

        //    return default;
        //}
    }
}
