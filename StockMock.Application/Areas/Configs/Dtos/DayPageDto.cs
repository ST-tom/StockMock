using LinqKit;
using StockMock.Application.Base;
using StockMock.Application.Extensions;
using StockMock.Domain.Entities.Configs;
using System.Linq.Expressions;

namespace StockMock.Application.Areas.Configs.Dtos
{
    public class DayPageDto : PageDto
    {
        public DateOnly? Date { get; set; }

        public bool? IsWorkDay { get; set; }

        public override Expression<Func<Day, bool>> GetWhereLamda()
        {
            var lamda = PredicateBuilder.New<Day>(true);

            if (Date.HasValue)
                lamda.And(e => e.Date == Date);

            if (IsWorkDay.HasValue)
                lamda.And(e => e.IsWorkDay == IsWorkDay);

            return lamda;
        }
    }

    public class DayPageDtoValidator : PageDtoValidator<DayPageDto>
    {
        public DayPageDtoValidator() : base()
        {
            RuleFor(v => v.Date)
                .MustDateRange();
        }
    }
}
