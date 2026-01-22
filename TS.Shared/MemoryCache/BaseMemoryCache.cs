using Microsoft.Extensions.Caching.Memory;
using TS.Shared.Extension;

namespace TS.Shared.MemoryCache
{
    public abstract class BaseMemoryCache<T, TKey> : IAsyncDisposable
        where TKey : notnull
    {
        /// <summary>
        /// 缓存对象
        /// </summary>
        protected readonly IMemoryCache _cache;

        public BaseMemoryCache()
        {
            _cache = new Microsoft.Extensions.Caching.Memory.MemoryCache(CacheOptions);
        }

        /// <summary>
        /// 缓存大小限制
        /// </summary>
        protected virtual int SizeLimet { get; set; } = 10240;

        /// <summary>
        /// 一份缓存所占大小
        /// </summary>
        protected virtual int Size { get; set; } = 1;

        /// <summary>
        /// 缓存滑动过期时间
        /// </summary>
        protected virtual TimeSpan SlidingExpireTime { get; set; } = TimeSpan.FromMinutes(30);

        /// <summary>
        /// 缓存绝对过期时间
        /// </summary>
        protected virtual TimeSpan AbsoluteExpireTime { get; set; } = TimeSpan.FromHours(6);

        /// <summary>
        /// 缓存配置
        /// </summary>
        protected virtual MemoryCacheOptions CacheOptions => DefaultCacheOptions;

        /// <summary>
        /// 默认缓存配置
        /// </summary>
        private MemoryCacheOptions DefaultCacheOptions => new()
        {
            ExpirationScanFrequency = TimeSpan.FromMinutes(5),
            SizeLimit = SizeLimet,
            CompactionPercentage = 0.05,//缓存回收百分比
        };

        /// <summary>
        /// 数据缓存配置
        /// </summary>
        protected virtual MemoryCacheEntryOptions CacheEntryOptions { get; set; }

        /// <summary>
        /// 获取缓存key的方法
        /// </summary>
        protected virtual Func<T, TKey>? GetKeyFunc { get; set; } = default;

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public virtual void Set(TKey key, T? value, MemoryCacheEntryOptions? options = default)
        {
            ArgumentNullException.ThrowIfNull(key);

            options ??= CacheEntryOptions;

            _cache.Set(key, value, options);
        }

        /// <summary>
        /// 批量设置缓存
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="options"></param>
        public virtual void Sets(Dictionary<TKey, T> dic, MemoryCacheEntryOptions? options = default)
        {
            if (!dic.HasData())
                return;

            options ??= CacheEntryOptions;
            dic.ForEach(item => _cache.Set(item.Key, item.Value, options));
        }

        /// <summary>
        /// 批量设置缓存
        /// </summary>
        /// <param name="list"></param>
        /// <param name="options"></param>
        public virtual void Sets(IEnumerable<T> array, MemoryCacheEntryOptions? options = default)
        {
            if (!array.HasData())
                return;

            ArgumentNullException.ThrowIfNull(GetKeyFunc);

            options ??= CacheEntryOptions;
            array.ForEach(item => _cache.Set(GetKeyFunc(item), item, options));
        }


        /// <summary>
        /// 设置缓存，相对过期时间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        /// <exception cref="ArgumentException"></exception>
        public virtual void Set(TKey key, T value, TimeSpan expireTime, int size = 1, CacheItemPriority priority = CacheItemPriority.Normal)
        {
            ArgumentNullException.ThrowIfNull(key);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(expireTime, TimeSpan.FromSeconds(60));

            Set(key, value, new MemoryCacheEntryOptions() { Size = size, Priority = priority, SlidingExpiration = expireTime });
        }

        /// <summary>
        /// 设置过期时间，绝对过期时间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteTime"></param>
        public virtual void Set(TKey key, T value, DateTime absoluteTime, int size = 1, CacheItemPriority priority = CacheItemPriority.Normal)
        {
            ArgumentNullException.ThrowIfNull(key);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(absoluteTime, DateTime.Now.AddSeconds(60));

            Set(key, value, new MemoryCacheEntryOptions() { Size = size, Priority = priority, AbsoluteExpiration = absoluteTime });
        }

        /// <summary>
        /// 设置缓存，相对过期时间，绝对过期时间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        /// <param name="absoluteTime"></param>
        /// <param name="size"></param>
        /// <param name="priority"></param>
        public virtual void Set(TKey key, T value, TimeSpan expireTime, DateTime absoluteTime, int size = 1, CacheItemPriority priority = CacheItemPriority.Normal)
        {
            ArgumentNullException.ThrowIfNull(key);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(expireTime, TimeSpan.FromSeconds(60));
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(absoluteTime, DateTime.Now.AddSeconds(60));
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(expireTime, absoluteTime - DateTime.Now);

            Set(key, value, new MemoryCacheEntryOptions() { Size = size, Priority = priority, AbsoluteExpiration = absoluteTime, SlidingExpiration = expireTime });
        }

        public virtual T? Get(TKey key)
        {
            if (_cache.TryGetValue(key, out T? value))
                return value;

            return default;
        }

        public void Remove(TKey key)
        {
            ArgumentNullException.ThrowIfNull(key);
            _cache.Remove(key);
        }

        public virtual ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }
    }
}
