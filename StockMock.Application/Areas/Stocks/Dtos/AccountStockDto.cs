using FluentValidation;
using StockMock.Application.Extensions;

namespace StockMock.Application.Areas.Stocks.Dtos
{
    public class AccountStockDto
    {
        public string Code { get; set; }

        public string Name { get; set; }
    }

    public class AccountStockDtoValidator : AbstractValidator<AccountStockDto>
    {
        public AccountStockDtoValidator(bool isValiteName = false)
        {
            RuleFor(v => v.Code)
                .MustStockCode();

            if (!isValiteName)
                RuleFor(v => v.Name)
                    .MustStockName();
        }
    }
}
