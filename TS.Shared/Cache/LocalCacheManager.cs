using Microsoft.Extensions.Caching.Memory;

namespace TS.Shared.Cache
{
    public class LocalCacheManager : IDisposable
    {
        private IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions() { ExpirationScanFrequency = TimeSpan.FromMinutes(5) });

        public T? Get<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            _cache.TryGetValue<T>(key, out var value);

            return value;
        }

        /// <summary>
        /// 设置缓存，永不过期。（不推荐）
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">缓存值</param>
        public void SetNotExpire<T>(string key, T value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            _cache.Set(key, value);
        }


        /// <summary>
        /// 设置缓存，滑动过期或者绝对时间过期。默认滑动过去2小时内无访问自动过期。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="span">时间</param>
        /// <param name="isAbsolutale">
        /// 否：滑动过期，超过一段时间不访问就会过期,一直访问就一直不过期；
        /// 是：绝对时间过期，从缓存开始持续指定的时间段后就过期,无论有没有持续的访问。
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Set<T>(string key, T value, TimeSpan span = default, bool isAbsolutale = false)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (span.Equals(default))
                span = TimeSpan.FromHours(2);

            _cache.Set(key, value, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = span
            });
        }


        /// <summary>
        /// 设置缓存，绝对时间过期+滑动过期:比如滑动过期设置半小时,绝对过期时间设置2个小时，那么缓存开始后只要半小时内没有访问就会立马过期,如果半小时内有访问就会向后顺延半小时，但最多只能缓存2个小时
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="slidingSpan">滑动过期时间</param>
        /// <param name="absoluteSpan">绝对过期时间</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Set<T>(string key, T value, TimeSpan slidingSpan, TimeSpan absoluteSpan)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            _cache.Set(key, value, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = slidingSpan,
                AbsoluteExpiration = DateTimeOffset.Now.AddMilliseconds(absoluteSpan.TotalMilliseconds)
            });
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">关键字</param>
        public void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            _cache.Remove(key);
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            _cache?.Dispose();
        }
    }
}

