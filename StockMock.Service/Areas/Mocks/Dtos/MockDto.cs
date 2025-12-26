using FluentValidation;
using StockMock.Service.FluentValidation;

namespace StockMock.Service.Areas.Mocks.Dtos
{
    public class MockDto
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 股票代码
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// 模拟起始日期
        /// </summary>
        public DateOnly MockDate { get; set; }
    }

    public class MockDtoValidator : AbstractValidator<MockDto>
    {
        public MockDtoValidator(bool isRequiredId = false)
        {
            if (isRequiredId)
            {
                RuleFor(v => v.Id)
                    .NotEmpty()
                    .WithMessage("编号不能为空");
            }
            else
            {
                RuleFor(v => v.StockCode)
                    .MustStockCode();
                
                RuleFor(v => v.MockDate)
                    .NotEmpty()
                    .WithMessage("模拟起始日期不能为空")
                    .MustDateRange();
            }
        }
    }
}
