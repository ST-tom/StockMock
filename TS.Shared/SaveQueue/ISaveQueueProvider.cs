namespace TS.Shared.SaveQueue
{
    /// <summary>
    /// 批量保存提供者接口
    /// </summary>
    /// <typeparam name="T">要保存的实体类型</typeparam>
    public interface ISaveQueueProvider<T>
    {
        /// <summary>
        /// 批量保存数据
        /// </summary>
        /// <param name="list">待保存的实体列表</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>保存结果（成功数量）</returns>
        Task<int> SaveBatchAsync(IEnumerable<T> list, CancellationToken cancellationToken = default);
    }
}
