using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace TS.Shared.Extension
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// 批量注册指定程序集中所有继承自 baseType 的类型为 Scoped 服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="baseType">基类类型</param>
        /// <param name="assembly">要扫描的程序集（默认：当前执行程序集）</param>
        /// <param name="registerAsBaseType">是否注册为基类类型（true：通过基类获取；false：通过自身类型获取）</param>
        public static IServiceCollection AddScopedByBaseType(this IServiceCollection services, Type baseType, Assembly? assembly = null, bool registerAsBaseType = false)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (baseType == null) throw new ArgumentNullException(nameof(baseType));

            // 默认扫描当前执行程序集
            assembly ??= Assembly.GetExecutingAssembly();

            // 筛选符合条件的类型：
            // 1. 非抽象类 2. 不是接口 3. 继承自 baseType 4. 不等于 baseType 本身
            var types = assembly.GetTypes().Where(type => !type.IsAbstract&& !type.IsInterface&& baseType.IsAssignableFrom(type)&& type != baseType).ToList();

            if (!types.Any())
                return services;

            // 批量注册为 Scoped
            foreach (var type in types)
            {
                if (registerAsBaseType)
                    // 注册为基类类型（可通过 baseType 解析）
                    services.AddScoped(baseType, type);
                else
                    // 注册为自身类型（需通过具体类型解析）
                    services.AddScoped(type);
            }

            return services;
        }

        /// <summary>
        /// 泛型版本：批量注册指定程序集中所有继承自 TBase 的类型为 Scoped 服务
        /// </summary>
        public static IServiceCollection AddScopedByBaseType<TBase>(this IServiceCollection services, Assembly? assembly = null, bool registerAsBaseType = false)
            where TBase : class
        {
            return services.AddScopedByBaseType(typeof(TBase), assembly, registerAsBaseType);
        }
    }
}
