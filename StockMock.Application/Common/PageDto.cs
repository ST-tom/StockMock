using FluentValidation;
using Newtonsoft.Json;

namespace StockMock.Application.Base
{
    public class PageDto
    {
        [JsonProperty("page_index")]
        public int PageIndex { get; private set; }

        [JsonProperty("page_size")]
        public int PageSize { get; private set; }

        [JsonProperty("order_by")]
        public string OrderBy { get; private set; }

        [JsonProperty("is_desc")]
        public bool IsDesc { get; private set; } = false;

        [JsonProperty("order_dic")]
        public Dictionary<string,bool> OrderDic { get; private set; }

        public virtual object? GetWhereLamda()
        {
            return default;
        }

        public virtual async Task<PageList<T>> LoadAsync<T>(IQueryable<T> source, CancellationToken cancellationToken)
        {
            return await PageList<T>.LoadAsync(source, this.PageIndex, this.PageSize, this.OrderBy, this.IsDesc, cancellationToken);
        }
    }

    public class PageDtoValidator<T> : AbstractValidator<T>
       where T : PageDto 
    {
        public PageDtoValidator()
        {
            RuleFor(v => v.PageIndex)
                .Must(v => v > 0)
                .WithMessage("页码必须大于0");

            RuleFor(v => v.PageIndex)
                .Must(v => v < int.MaxValue)
                .WithMessage("页码过大");

            RuleFor(v => v.PageSize)
                .Must(v => v > 0)
            .WithMessage("页条数必须大于0");

            RuleFor(v => v.PageSize)
                .Must(v => v <= 1000)
                .WithMessage("页条数最大不超过1000");
        }
    }
}
