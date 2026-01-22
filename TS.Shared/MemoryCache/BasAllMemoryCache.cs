using Microsoft.Extensions.Caching.Memory;
using TS.Shared.Extension;

namespace TS.Shared.MemoryCache
{
    public abstract class BasAllMemoryCache<T, Tkey> : BaseMemoryCache<T, Tkey>
        where Tkey : notnull
    {
        public BasAllMemoryCache() : base()
        {
            ArgumentNullException.ThrowIfNull(QueryDataFuncAsync);
            ArgumentNullException.ThrowIfNull(GetKeyFunc);

            CacheEntryOptions = AllRefreshCahceEntryOptions;
        }

        /// <summary>
        /// 缓存最后全量刷新时间
        /// </summary>
        public DateTime? LastAllRefreshTime { get; set; }

        /// <summary>
        /// 全量刷新缓存配置
        /// </summary>
        private MemoryCacheEntryOptions AllRefreshCahceEntryOptions => new()
        {
            Size = Size, //每份缓存所占的大小      
            Priority = CacheItemPriority.Normal,
            AbsoluteExpirationRelativeToNow = AbsoluteExpireTime,
        };

        /// <summary>
        /// 全量刷新缓存方法
        /// </summary>
        protected virtual Func<CancellationToken, Task<IEnumerable<T>>> QueryDataFuncAsync { get; set; }

        /// <summary>
        /// 全量刷新缓存任务
        /// </summary>
        private Task? _refreshTask = default;

        /// <summary>
        /// 全量刷新缓存任务取消事件
        /// </summary>
        private CancellationTokenSource? _taskTokenSource = default;

        /// <summary>
        /// 全量刷新缓存任务定时器
        /// </summary>
        private PeriodicTimer? _taskTimer = default;

        /// <summary>
        /// 启动定时全量刷新缓存
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _taskTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var data = await QueryDataFuncAsync!(_taskTokenSource.Token);
            if (data.HasData())
                data!.ForEach(item => Set(GetKeyFunc!(item), item));

            _refreshTask = Task.Run(async () =>
            {
                try
                {
                    _taskTimer = new(AbsoluteExpireTime.Add(TimeSpan.FromMinutes(-5)));
                    while (await _taskTimer.WaitForNextTickAsync(_taskTokenSource.Token) && !_taskTokenSource.Token.IsCancellationRequested)
                    {
                        var data = await QueryDataFuncAsync(_taskTokenSource.Token);
                        if (data.HasData())
                        {
                            data!.ForEach(item => Set(GetKeyFunc!(item), item));
                            LastAllRefreshTime = DateTime.Now;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    //终止不做任何处理
                }
            }, _taskTokenSource.Token);
        }

        /// <summary>
        /// 停止定时全量刷新缓存
        /// </summary>
        /// <returns></returns>
        public async Task StopAsync()
        {
            if (_taskTokenSource != default)
                await _taskTokenSource!.CancelAsync();

            if (_refreshTask != default)
                await _refreshTask;
        }

        /// <summary>
        /// 刷新所有缓存
        /// </summary>
        /// <returns></returns>
        public async Task RefreshAllAsync()
        {
            //简单控制 只有定时任务和这里刷新能正常获取即可
            var data = await QueryDataFuncAsync(CancellationToken.None);
            if (data.HasData())
            {
                data!.ForEach(item => Set(GetKeyFunc!(item), item));
                LastAllRefreshTime = DateTime.Now;
            }
        }

        public override async ValueTask DisposeAsync()
        {
            await StopAsync();

            _taskTimer?.Dispose();
            _taskTokenSource?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
