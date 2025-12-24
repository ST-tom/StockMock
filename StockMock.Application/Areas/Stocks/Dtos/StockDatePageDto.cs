using LinqKit;
using StockMock.Application.Base;
using StockMock.Application.Extensions;
using StockMock.Domain.Entities.Stocks;
using StockMock.Infrastructure.Extensions;
using System.Linq.Expressions;

namespace StockMock.Application.Areas.Stocks.Dtos
{
    public class StockDatePageDto : PageDto
    {
        public string StockCode { get; set; }

        public string StockName { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public override Expression<Func<StockDate, bool>> GetWhereLamda()
        {
            var lamda = PredicateBuilder.New<StockDate>(true);

            if (StockCode.IsNotNullOrEmpty())
                lamda.And(e => e.StockCode.Contains(StockCode));

            if (StockName.IsNotNullOrEmpty())
                lamda.And(e => e.StockName.Contains(StockName));

            if (StartDate.HasValue)
                lamda.And(e => e.Date >= StartDate);

            if(EndDate.HasValue)
                lamda.And(e => e.Date <= EndDate);

            return lamda;
        }
    }

    public class StockDatePageDtoValidator : PageDtoValidator<StockDatePageDto>
    {
        public StockDatePageDtoValidator() : base()
        {
            RuleFor(e => e.StockCode)
                .MustStockCodeLength();

            RuleFor(e => e.StockName)
                .MustStockNameLength();

            RuleFor(e => e.StartDate)
                .MustDateRange();

            RuleFor(e => e.EndDate)
                .MustDateRange();
        }
    }
}
