using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using TS.Shared.Query;

namespace StockMock.Service.FluentValidation
{
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
