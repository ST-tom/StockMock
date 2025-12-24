using FluentValidation;
using StockMock.Application.Extensions;
using StockMock.Domain.Entities.Mocks;

namespace StockMock.Application.Areas.Mocks.Dtos
{
    public class MockDateDto
    {
        /// <summary>
        /// 模拟Id
        /// </summary>
        public long MockId { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateOnly Date { get; set; }

        /// <summary>
        /// 持仓数量
        /// </summary>
        public int PositionQuantity { get; set; }

        /// <summary>
        /// 仓位比例类型
        /// </summary>
        public PositionRateType PositionRateType { get; set; }

        /// <summary>
        /// 预测涨幅
        /// </summary>
        public PredictionType Prediction{ get; set; }
    }

    public class MockDateDtoValidator : AbstractValidator<MockDateDto>
    {
        public MockDateDtoValidator()
        {
            RuleFor(v => v.MockId)
                .NotEmpty()
                .WithMessage("模拟编号不能为空");

            RuleFor(v => v.Date)
                .NotEmpty()
                .WithMessage("日期不能为空")
                .MustDateRange();

            RuleFor(v => v.PositionQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("持仓数量必须大于等于0");
        }
    }
}
