using LinqKit;
using StockMock.Core.Stocks;
using StockMock.Service.FluentValidation;
using System.Linq.Expressions;
using TS.Shared.Extension;
using TS.Shared.Query;

namespace StockMock.Service.Areas.Stocks.Dtos
{
    public class AccountStockPageDto : PageDto
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public bool? Enabled { get; set; }

        public override Expression<Func<AccountStock, bool>> GetWhereLamda()
        {
            var lamda = PredicateBuilder.New<AccountStock>(true);

            if (Code.IsNotNullOrEmpty())
                lamda.And(e => e.StockCode.Contains(Code!));

            if(Name.IsNotNullOrEmpty())
                lamda.And(e => e.StockName.Contains(Name!));

            if(Enabled.HasValue)
                lamda.And(e => e.IsEnabled == Enabled);

            return lamda;
        }
    }

    public class AccountStockPageDtoValidator : PageDtoValidator<AccountStockPageDto>
    {
        public AccountStockPageDtoValidator() : base() {
            RuleFor(e => e.Code)
                .MustStockCodeLength();

            RuleFor(e => e.Name)
                .MustStockNameLength();
        }
    }
}
