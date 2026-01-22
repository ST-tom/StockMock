using Microsoft.Extensions.Caching.Memory;

namespace TS.Shared.MemoryCache
{
    public abstract class BaseActiveMemoryCache<T, TKey> : BaseMemoryCache<T, TKey>
        where TKey : notnull
    {
        public BaseActiveMemoryCache() : base()
        {
            ArgumentNullException.ThrowIfNull(QueryKeyDataFunc);

            CacheEntryOptions = AbsoluteAndSlidingCahceEntryOptions;
        }

        protected virtual bool IsAutoRefreshExpired { get; set; } = false;

        /// <summary>
        /// 绝对和滑动过期时间缓存配置
        /// </summary>
        private MemoryCacheEntryOptions AbsoluteAndSlidingCahceEntryOptions
        {
            get
            {
                MemoryCacheEntryOptions options = new()
                {
                    Size = Size, //每份缓存所占的大小      
                    Priority = CacheItemPriority.Normal,
                    SlidingExpiration = SlidingExpireTime,
                    AbsoluteExpirationRelativeToNow = AbsoluteExpireTime,
                };

                if (IsAutoRefreshExpired && QueryKeyDataFunc != default)
                {
                    options.RegisterPostEvictionCallback(async (key, value, reason, state) =>
                    {
                        var tkey = (TKey)key;
                        var data = await QueryKeyDataFunc(tkey);
                        Set(tkey, data);
                    });
                }

                return options;
            }
        }

        /// <summary>
        /// 根据Key获取缓存数据方法
        /// </summary>
        protected virtual Func<TKey, Task<T?>> QueryKeyDataFunc { get; set; }

        /// <summary>
        /// 根据Key获取缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="queryKeyDataAsyncFunc"></param>
        /// <returns></returns>
        public async ValueTask<T?> GetAsync(TKey key, Func<TKey, Task<T?>> queryKeyDataAsyncFunc)
        {
            ArgumentNullException.ThrowIfNull(queryKeyDataAsyncFunc);

            if (_cache.TryGetValue(key, out T? value))
                return value;

            QueryKeyDataFunc = queryKeyDataAsyncFunc;
            value = await QueryKeyDataFunc(key);
            Set(key, value);
            return value;
        }
    }
}
