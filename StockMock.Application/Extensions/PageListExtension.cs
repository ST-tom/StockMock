using AutoMapper;
using StockMock.Application.Base;

namespace StockMock.Application.Extensions
{
    public static class PageListExtension
    {
        public static PageList<T2> ProjectTo<T1, T2>(this PageList<T1> pageList, IMapper mapper)
        {
            var items = pageList.Items.Select(e => mapper.Map<T2>(e));
            return new PageList<T2>(items.ToList(), pageList.PageIndex, pageList.PageSize, pageList.TotalCount);
        }
    }
}
