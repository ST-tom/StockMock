using LinqKit;
using StockMock.Core.Configs;
using StockMock.Service.FluentValidation;
using System.Linq.Expressions;
using TS.Shared.Query;

namespace StockMock.Service.Areas.Configs.Dtos
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
