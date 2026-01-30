using System.Collections.Concurrent;
using TS.Shared.Json;
using TS.Shared.Util;

namespace TS.Shared.SaveQueue
{
    public class SaveQueueManager<T>(ISaveQueueProvider<T> provider) : IAsyncDisposable
        where T : class
    {
        /// <summary>
        /// 定时器
        /// </summary>
        private PeriodicTimer? _periodicTimer = default;

        /// <summary>
        /// 取消令牌
        /// </summary>
        private CancellationTokenSource? _taskTokenSource = default;

        /// <summary>
        /// 保存任务
        /// </summary>
        private Task? _saveTask = default;

        /// <summary>
        /// 批量保存的队列
        /// </summary>
        private readonly ConcurrentQueue<T> _queue = new();

        /// <summary>
        /// 批量保存的队列
        /// </summary>
        private readonly ConcurrentQueue<(T data, int retryCount)> _retryQueue = new();

        /// <summary>
        /// 批次大小
        /// </summary>
        protected virtual int BatchSize { get; set; } = 1000;

        /// <summary>
        /// 重试限制
        /// </summary>
        protected virtual int RetryLimit { get; set; } = 3;

        /// <summary>
        /// 最大队列大小
        /// </summary>
        protected virtual int? MaxQueueSize { get; set; } = null;

        /// <summary>
        /// 队列扫描间隔
        /// </summary>
        protected virtual TimeSpan ScanInterval { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// 保存提供者
        /// </summary>
        protected readonly ISaveQueueProvider<T> provider = provider ?? throw new ArgumentNullException(nameof(provider));

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="record"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<bool> Insert(T? record)
        {
            if (record == null)
                return true;

            if (_queue.Count >= MaxQueueSize)
                return false;

            _queue.Enqueue(record);

            if (_queue.Count >= BatchSize)
                await ExcuteSaveAsync();

            return true;
        }

        /// <summary>
        /// 启动自动保存任务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            //已启动task任务，无需重复启动
            if (_saveTask != null)
                return;

            _saveTask = WhileRunTask(cancellationToken);           
        }

        /// <summary>
        /// 自动保存任务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task WhileRunTask(CancellationToken cancellationToken)
        {
            _taskTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _periodicTimer = new PeriodicTimer(ScanInterval);
            var linkedToken = _taskTokenSource.Token;

            try
            {
                //固定间隔执行
                while (await _periodicTimer.WaitForNextTickAsync(linkedToken) && !linkedToken.IsCancellationRequested)
                {
                    var excuteTask = ExcuteSaveAsync();
                    var retryTask = RetrySaveAsync();

                    await Task.WhenAll(excuteTask, retryTask);
                }
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == linkedToken)
            {
                
            }
            catch (OperationCanceledException)
            {
                //外部取消，处理剩余队列数据即可，不抛出异常
                await SaveAllQueueAsync();
            }
            catch (Exception)
            {
                //未知异常，处理剩余队列数据，抛出异常
                await SaveAllQueueAsync();
                throw;
            }
        }

        /// <summary>
        /// 停止自动保存任务
        /// </summary>
        /// <returns></returns>
        public async Task StopAsync()
        {
            _taskTokenSource?.Cancel();

            if (_saveTask != null)
            {
                await _saveTask;
                _saveTask = null;
            }

            await SaveAllQueueAsync();
        }

        /// <summary>
        /// 执行保存
        /// </summary>
        /// <returns></returns>
        private async Task ExcuteSaveAsync()
        {
            List<T> saveList = [];
            while (saveList.Count < BatchSize && _queue.TryDequeue(out T? item))
            {
                if (item != null)
                    saveList.Add(item);
            }

            if (saveList.Count == 0)
                return;

            try
            {
                await provider.SaveBatchAsync(saveList);
            }
            catch (Exception)
            {
                saveList.ForEach(e => _retryQueue.Enqueue((e, 0)));
            }
        }

        /// <summary>
        /// 重试队列保存
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task RetrySaveAsync()
        {
            List<(T data, int retryCount)> retryList = [];
            while (retryList.Count < BatchSize && _retryQueue.TryDequeue(out var item))
                retryList.Add(item);

            if (retryList.Count == 0)
                return;

            List<T> errList = [];
            try
            {
                await provider.SaveBatchAsync(retryList.Select(e => e.data));
            }
            catch (Exception)
            {
                //可能是某条异常数据导致保存，需要一条条保存找出异常数据
                retryList.ForEach(async item =>
                {
                    try
                    {
                        await provider.SaveBatchAsync([item.data]);
                    }
                    catch (Exception)
                    {
                        item.retryCount++;
                        if (item.retryCount < RetryLimit)
                            _retryQueue.Enqueue(item);
                        else
                            errList.Add(item.data);
                    }
                });
            }
            finally
            {
                if (errList.Count > 0)
                    await SaveToFile(errList);               
            }
        }

        /// <summary>
        /// 保存所有队列数据
        /// </summary>
        /// <returns></returns>
        private async Task SaveAllQueueAsync()
        {
            List<T> saveList = [];
            while (_queue.TryDequeue(out T? item))
            {
                if (item != null)
                    saveList.Add(item);

                if (saveList.Count < BatchSize)
                    continue;

                try
                {
                    await provider.SaveBatchAsync(saveList);
                }
                catch (Exception)
                {
                    await SaveToFile(saveList);
                }
            }

            while (_retryQueue.TryDequeue(out var node))
            {
                var item = node.data;
                if (item != null)
                    saveList.Add(item);

                if (saveList.Count < BatchSize)
                    continue;

                try
                {
                    await provider.SaveBatchAsync(saveList);
                }
                catch (Exception)
                {
                    await SaveToFile(saveList);
                }
            }

            if (saveList.Count > 0)
            {
                try
                {
                    await provider.SaveBatchAsync(saveList);
                }
                catch (Exception)
                {
                    await SaveToFile(saveList);
                }
            }
        }

        private static async Task SaveToFile(IEnumerable<T> saveList) => await FileUtil.SaveFileAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", "SaveQueue_Error"), FileUtil.GetExtDateTimeGuidFileName($"{typeof(T).Name}"), saveList.ToJsonString(), cancellationToken: CancellationToken.None);

        public async ValueTask DisposeAsync()
        {
            await StopAsync();
            _taskTokenSource?.Dispose();
            _periodicTimer?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
