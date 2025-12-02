using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StockMock.Application.Extensions;
using StockMock.Infrastructure.Extensions;

namespace StockMock.Application.Base
{
    [Serializable]
    public class PageList<T>
    {
        [JsonProperty("page_index")]
        public int PageIndex { get; private set; }

        [JsonProperty("page_size")]
        public int PageSize { get; private set; }

        [JsonProperty("total_count")]
        public int TotalCount { get; private set; }

        [JsonProperty("total_page")] 
        public int TotalPage { get; private set; }

        [JsonProperty("has_preview")]
        public bool HasPreview => PageIndex > 1 && TotalPage >=2;

        [JsonProperty("has_next")]
        public bool HasNext => PageIndex < TotalPage;

        [JsonProperty("items")]
        public IReadOnlyCollection<T> Items { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="totalCount">Total count</param>
        public PageList(IReadOnlyCollection<T> source, int pageIndex, int pageSize, int totalCount)
        {
            TotalCount = totalCount;
            TotalPage = (TotalCount + pageSize - 1) / pageSize;
            PageSize = pageSize;
            PageIndex = pageIndex;
            Items = source;
        }

        public static async Task<PageList<T>> LoadAsync(IQueryable<T> source, int pageIndex, int pageSize, string? orderBy = null, bool isDesc = false,CancellationToken cancellationToken = default)
        {
            var count = await source.CountAsync(cancellationToken);
            source = source.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            if(orderBy.IsNotNullOrEmpty())
                source = source.OrderyByField(orderBy!, isDesc);

            var list = await source.ToListAsync(cancellationToken);

            return new PageList<T>(list, pageIndex, pageSize, count);
        }

        public static async Task<PageList<T>> LoadAsync(IQueryable<T> source, int pageIndex, int pageSize, Dictionary<string, bool> orderDic, CancellationToken cancellationToken = default)
        {
            var count = await source.CountAsync(cancellationToken);
            source = source.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            
            orderDic.ForEach(item => source = source.OrderyByField(item.Key, item.Value));            

            var list = await source.ToListAsync(cancellationToken);

            return new PageList<T>(list, pageIndex, pageSize, count);
        }
    }
}
