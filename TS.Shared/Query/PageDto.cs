using System.Text.Json.Serialization;

namespace TS.Shared.Query
{
    public class PageDto
    {
        [JsonPropertyName("page_index")]
        public int PageIndex { get; private set; }

        [JsonPropertyName("page_size")]
        public int PageSize { get; private set; }

        [JsonPropertyName("order_by")]
        public string? OrderBy { get; private set; }

        [JsonPropertyName("is_desc")]
        public bool IsDesc { get; private set; } = false;

        [JsonPropertyName("order_dic")]
        public Dictionary<string,bool>? OrderDic { get; private set; }

        public virtual object? GetWhereLamda()
        {
            return default;
        }

        public virtual async Task<PageList<T>> LoadAsync<T>(IQueryable<T> source, CancellationToken cancellationToken)
        {
            return await PageList<T>.LoadAsync(source, PageIndex, PageSize, OrderBy, IsDesc, cancellationToken);
        }
    }
}
