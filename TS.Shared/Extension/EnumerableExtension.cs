namespace TS.Shared.Extension
{
    public static class EnumerableExtension
    {
        // <summary>
        /// 对 IEnumerable<T>的每个元素执行指定操作
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="source">要遍历的 IEnumerable<T>集合</param>
        /// <param name="action">对每个元素执行的操作（Action<T>委托）</param>
        /// <exception cref="ArgumentNullException">当 source 或 action 为 null 时抛出</exception>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null || !source.Any())
                return;

            if (action == null)
                throw new ArgumentNullException(nameof(action), "执行的操作不能为 null");

            // 遍历集合，对每个元素执行 action
            foreach (T item in source)
            {
                action(item);
            }
        }
    }
}

