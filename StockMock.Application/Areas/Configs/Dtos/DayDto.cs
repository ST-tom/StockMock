using FluentValidation;
using StockMock.Application.Extensions;

namespace StockMock.Application.Areas.Configs.Dtos
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
