using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace TS.Shared.Extension
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// 批量注册指定程序集中所有继承自 baseType 的类型为 Scoped 服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="prefix">程序集前缀</param>
        /// <param name="baseType"></param>
        /// <param name="assembly"></param>
        /// <param name="registerAsBaseType"></param>
        /// <returns></returns>
        public static IServiceCollection AddScopedByBaseType(this IServiceCollection services, Type baseType, Assembly? assembly = default, bool registerAsBaseType = false)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(baseType);

            assembly ??= Assembly.GetExecutingAssembly();

            // 非抽象类 不是接口 继承自 baseType 不等于 baseType 本身
            var types = assembly.GetTypes().Where(type => !type.IsAbstract && !type.IsInterface && baseType.IsAssignableFrom(type) && type != baseType).ToList();

            if (types.Count == 0)
                return services;

            foreach (var type in types)
            {
                if (registerAsBaseType)
                    services.AddScoped(baseType, type);
                else
                    services.AddScoped(type);
            }

            return services;
        }

        /// <summary>
        /// 批量注册指定程序集中所有继承自 baseType 的类型为 Scoped 服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="prefix">程序集前缀</param>
        /// <param name="baseType"></param>
        /// <param name="assembly"></param>
        /// <param name="registerAsBaseType"></param>
        /// <returns></returns>
        public static IServiceCollection AddScopedByBaseType<T>(this IServiceCollection services, Assembly? assembly = default, bool registerAsBaseType = false) where T : class
            => services.AddScopedByBaseType(typeof(T), assembly, registerAsBaseType);

        /// <summary>
        /// 批量注册指定程序集中所有继承自 baseType 的类型为 Scoped 服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="prefix">程序集前缀</param>
        /// <param name="baseType"></param>
        /// <param name="assembly"></param>
        /// <param name="registerAsBaseType"></param>
        /// <returns></returns>
        public static IServiceCollection AddScopedByBaseTypeByPrefix(this IServiceCollection services, string prefix, Type baseType, bool registerAsBaseType = false)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(baseType);

            var assemblys = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName?.StartsWith(prefix) == true).ToList();

            foreach (var assembly in assemblys)
            {
                // 非抽象类 不是接口 继承自 baseType 不等于 baseType 本身
                var types = assembly.GetTypes().Where(type => !type.IsAbstract && !type.IsInterface && baseType.IsAssignableFrom(type) && type != baseType).ToList();

                if (types.Count == 0)
                    continue;

                foreach (var type in types)
                {
                    if (registerAsBaseType)
                        services.AddScoped(baseType, type);
                    else
                        services.AddScoped(type);
                }
            }

            return services;
        }

        /// <summary>
        /// 泛型版本：批量注册指定程序集中所有继承自 TBase 的类型为 Scoped 服务
        /// </summary>
        public static IServiceCollection AddScopedByBaseTypeByPrefix<TBase>(this IServiceCollection services, string prefix, bool registerAsBaseType = false) where TBase : class
            => services.AddScopedByBaseTypeByPrefix(prefix, typeof(TBase), registerAsBaseType);
    }
}
