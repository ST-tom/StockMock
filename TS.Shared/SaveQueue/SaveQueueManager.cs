using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using TS.Shared.Json;
using TS.Shared.Query;
using TS.Shared.Util;

namespace TS.Shared.SaveQueue
{
    public class SaveQueueOptions
    {
        /// <summary>
        /// 批次大小
        /// </summary>
        public int BatchSize { get; set; } = 1000;

        /// <summary>
        /// 重试限制
        /// </summary>
        public int RetryLimit { get; set; } = 3;

        /// <summary>
        /// 最大队列大小
        /// </summary>
        public int? MaxQueueSize { get; set; } = null;

        /// <summary>
        /// 队列扫描间隔
        /// </summary>
        public TimeSpan ScanInterval { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// 失败数据文件保存路径
        /// </summary>
        public string FailureFilePath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", "SaveQueue_Error");

        public static SaveQueueOptions DefaultOptions => new()
        {
            MaxQueueSize = 20000,
        };
    }

    public class SaveQueueManager<T, TType>(ILogger<TType> logger, ISaveQueueProvider<T> provider) : IAsyncDisposable
        where T : class
    {
        #region 内部字段

        private readonly ILogger<TType> _logger = logger;

        protected readonly ISaveQueueProvider<T> provider = provider ?? throw new ArgumentNullException(nameof(provider));

        private readonly SaveQueueOptions _saveQueueOptions = SaveQueueOptions.DefaultOptions;

        private readonly Lock _lock = new();
   
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
        /// 批量重试的队列
        /// </summary>
        private readonly ConcurrentQueue<(T data, int retryCount)> _retryQueue = new();

        #endregion

        #region 构造函数

        public SaveQueueManager(ILogger<TType> logger, ISaveQueueProvider<T> provider, SaveQueueOptions saveQueueOptions) : this(logger, provider)
        {
            _saveQueueOptions = saveQueueOptions;
        }

        #endregion

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

            if (_queue.Count >= _saveQueueOptions.MaxQueueSize)
                await ExcuteSaveAsync();

            if (_queue.Count >= _saveQueueOptions.MaxQueueSize)
            {
                return false;
            }

            _queue.Enqueue(record);

            if (_queue.Count >= _saveQueueOptions.BatchSize)
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
            lock (_lock)
            {
                //已启动task任务，无需重复启动
                if (_saveTask != null)
                    return;
            }

            _taskTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _periodicTimer = new PeriodicTimer(_saveQueueOptions.ScanInterval);
            var linkedToken = _taskTokenSource.Token;

            _saveTask = AutoSaveTask(linkedToken);
            await _saveTask.ConfigureAwait(false);
        }

        /// <summary>
        /// 自动保存任务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task AutoSaveTask(CancellationToken cancellationToken)
        {
            try
            {
                //固定间隔执行
                while (await _periodicTimer!.WaitForNextTickAsync(cancellationToken))
                {
                    var excuteTask = ExcuteSaveAsync();
                    var retryTask = ExcuteRetrySaveAsync();

                    await Task.WhenAll(excuteTask, retryTask);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await SaveAllQueueAsync();
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
        }

        /// <summary>
        /// 执行保存
        /// </summary>
        /// <returns></returns>
        private async Task ExcuteSaveAsync()
        {
            List<T> saveList = [];
            while (saveList.Count < _saveQueueOptions.BatchSize && _queue.TryDequeue(out T? item))
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
        private async Task ExcuteRetrySaveAsync()
        {
            List<(T data, int retryCount)> retryList = [];
            while (retryList.Count < _saveQueueOptions.BatchSize && _retryQueue.TryDequeue(out var item))
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

                var tasks = retryList.Select(async item =>
                {
                    try
                    {
                        await provider.SaveBatchAsync([item.data]);
                    }
                    catch (Exception)
                    {
                        item.retryCount++;
                        if (item.retryCount < _saveQueueOptions.RetryLimit)
                            _retryQueue.Enqueue(item);
                        else
                            errList.Add(item.data);
                    }
                });
                await Task.WhenAll(tasks);
            }
            finally
            {
                if (errList.Count > 0)
                {
                    try
                    {
                        await SaveToFile(errList);
                    }
                    catch
                    {
                        _logger.LogError("批量保存队列{TType}保存失败写入重试文件也失败，异常数据：{errList}", nameof(TType), errList.ToJsonString());
                    }
                }
            }
        }

        /// <summary>
        /// 保存队列所有剩余数据
        /// </summary>
        /// <returns></returns>
        private async Task SaveAllQueueAsync()
        {
            List<T> saveList = [];
            while (_queue.TryDequeue(out T? item))
            {
                if (item != null)
                    saveList.Add(item);

                if (saveList.Count < _saveQueueOptions.BatchSize)
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

                if (saveList.Count < _saveQueueOptions.BatchSize)
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

        private async Task SaveToFile(IEnumerable<T> saveList) => await FileUtil.SaveFileAsync(_saveQueueOptions.FailureFilePath, FileUtil.GetExtDateTimeGuidFileName($"{typeof(T).Name}"), saveList.ToJsonString(), cancellationToken: CancellationToken.None);

        public async ValueTask DisposeAsync()
        {
            await StopAsync();
            _taskTokenSource?.Dispose();
            _periodicTimer?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
