using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using TS.Shared.Json;
using TS.Shared.Util;

namespace TS.Shared.SaveQueue
{
    public abstract class SaveQueueManager<T>(
        ISaveQueueProvider<T> provider,
        Logger<SaveQueueManager<T>> logger,
        int batchSize = 100,
        int maxWaitSeconds = 5,
        int retryCount = 3) : IAsyncDisposable
    {
        #region 配置

        /// <summary>
        /// 批量保存的批次大小
        /// </summary>
        protected int _batchSize = batchSize > 0 ? batchSize : 100;

        /// <summary>
        /// 批量保存的最大间隔时间
        /// </summary>
        protected TimeSpan _maxTimeSpan = TimeSpan.FromSeconds(maxWaitSeconds > 0 ? maxWaitSeconds : 5);

        /// <summary>
        /// 重试次数
        /// </summary>
        protected int _retryCount = retryCount >= 0 ? retryCount : 3;

        /// <summary>
        /// 批量保存的队列
        /// </summary>
        protected ConcurrentQueue<T> _queue = new();

        /// <summary>
        /// 保存提供者
        /// </summary>
        protected ISaveQueueProvider<T> _provider = provider ?? throw new ArgumentNullException(nameof(provider));

        /// <summary>
        /// 日志记录器
        /// </summary>
        private Logger<SaveQueueManager<T>> _logger = logger;

        /// <summary>
        /// 取消令牌
        /// </summary>
        private CancellationTokenSource? _cancellationTokenSource = default;

        /// <summary>
        /// 任务
        /// </summary>
        private Task? _task = default;

        #endregion

        /// <summary>
        /// 记录入队
        /// </summary>
        /// <param name="record"></param>
        public void Insert(T record)
        {
            if (record == null || record.Equals(default))
                return;

            _queue.Enqueue(record);
        }

        public async Task Start(CancellationToken cancellationToken = default)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _task = TaskAsync(_cancellationTokenSource.Token);
        }

        private async Task TaskAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                List<T> itemsToSave = [];
                try
                {
                    while (_queue.TryDequeue(out var item) && itemsToSave.Count < _batchSize)
                    {
                        itemsToSave.Add(item);
                    }

                    if (itemsToSave.Count == 0)
                    {
                        await Task.Delay(_maxTimeSpan, cancellationToken);
                        continue;
                    }

                    await RetryUtil.RunAsync(() => _provider.SaveBatchAsync(itemsToSave, cancellationToken), cancellationToken, _retryCount);

                    itemsToSave.Clear();
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[{typeof(T).Name}] 异步批量保存失败，错误信息：{ex.Message}", typeof(T).Name, ex.Message);

                    if (itemsToSave.Count > 0)
                        _logger.LogError("批量保存失败数据：{itemsToSave}", itemsToSave.ToJsonString());
                            
                    await Task.Delay(_maxTimeSpan, cancellationToken);
                }

                await Task.Delay(_maxTimeSpan, cancellationToken);
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        private async Task StopAsync()
        {
            _cancellationTokenSource?.Cancel();

            if (_task != null)
            {
                await _task;
            }

            List<T> remainingItems = [];
            while (_queue.TryDequeue(out var item))
            {
                remainingItems.Add(item);
            }

            if (remainingItems.Count > 0)
            {
                await RetryUtil.RunAsync(() => _provider.SaveBatchAsync(remainingItems));
                Console.WriteLine($"[{typeof(T).Name}] 停止时处理剩余数据：{remainingItems.Count}条");
            }
        }

        /// <summary>
        /// 异步释放资源
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await StopAsync();
            _cancellationTokenSource?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
