using FluentValidation;
using StockMock.Core.Stocks;
using StockMock.Service.FluentValidation;

namespace StockMock.Service.Areas.Stocks.Dtos
{
    public class AccountStockDto
    {
        /// <summary>
        /// 股票代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 股票名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 板块类型
        /// </summary>
        public BoardType BoardType { get; set; }
    }

    public class AccountStockDtoValidator : AbstractValidator<AccountStockDto>
    {
        public AccountStockDtoValidator(bool isNotOnlyCode = false)
        {
            RuleFor(v => v.Code)
                .MustStockCode();

            if (!isNotOnlyCode)
            {
                RuleFor(v => v.Name)
                    .MustStockName();
            }
        }
    }
}
