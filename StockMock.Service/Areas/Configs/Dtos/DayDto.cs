using FluentValidation;
using StockMock.Service.FluentValidation;

namespace StockMock.Service.Areas.Configs.Dtos
{
    public class DayDto
    {
        public DateOnly Date { get; set; }

        public bool IsWorkDay { get; set; }
    }

    public class DayDtoValidator : AbstractValidator<DayDto>
    {
        public DayDtoValidator()
        {
            RuleFor(v => v.Date)
                .MustDateRange();
        }
    }
}
