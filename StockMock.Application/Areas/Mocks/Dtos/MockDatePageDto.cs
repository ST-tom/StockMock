using LinqKit;
using StockMock.Application.Base;
using StockMock.Application.Extensions;
using StockMock.Domain.Entities.Mocks;
using StockMock.Infrastructure.Extensions;
using System.Linq.Expressions;

namespace StockMock.Application.Areas.Mocks.Dtos
{
    public class MockDatePageDto : PageDto
    {
        /// <summary>
        /// 编号
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 股票代码
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// 起始日期
        /// </summary>
        public DateOnly? StartDate { get; set; }

        /// <summary>
        /// 截止日期
        /// </summary>
        public DateOnly? EndDate { get; set; }

        public override Expression<Func<MockDate, bool>> GetWhereLamda()
        {
            var lamda = PredicateBuilder.New<MockDate>(true);

            if (StockCode.IsNotNullOrEmpty())
                lamda.And(e => e.Mock.StockCode.Equals(StockCode));

            if (StartDate.HasValue)
                lamda.And(e => e.Date >= StartDate);

            if (EndDate.HasValue)
                lamda.And(e => e.Date <= EndDate);

            return lamda;
        }
    }

    public class MockDatePageDtoValidator : PageDtoValidator<MockDatePageDto>
    {
        public MockDatePageDtoValidator() : base()
        {
            RuleFor(e => e.StockCode)
                .MustStockCode();

            RuleFor(e => e.StartDate)
                .MustDateRange();

            RuleFor(e => e.EndDate)
                .MustDateRange();
        }
    }
}
