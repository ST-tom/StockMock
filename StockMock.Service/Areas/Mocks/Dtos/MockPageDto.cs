using LinqKit;
using StockMock.Core.Mocks;
using StockMock.Service.FluentValidation;
using System.Linq.Expressions;
using TS.Shared.Extension;
using TS.Shared.Query;

namespace StockMock.Service.Areas.Mocks.Dtos
{
    public class MockPageDto : PageDto
    {       
        /// <summary>
        /// 编号
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 股票代码
        /// </summary>
        public string StockCode { get; set; } = string.Empty;

        /// <summary>
        /// 股票名称
        /// </summary>
        public string StockName { get; set; } = string.Empty;

        /// <summary>
        /// 模拟起始日期
        /// </summary>
        public DateOnly? StartDate { get; set; }

        /// <summary>
        /// 模拟截止日期
        /// </summary>
        public DateOnly? EndDate { get; set; }

        public override Expression<Func<Mock, bool>> GetWhereLamda()
        {
            var lamda = PredicateBuilder.New<Mock>(true);

            if (StockCode.IsNotNullOrEmpty())
                lamda.And(e => e.StockCode.Contains(StockCode));

            if (StockName.IsNotNullOrEmpty())
                lamda.And(e => e.StockName.Contains(StockName));

            if (StartDate.HasValue)
                lamda.And(e => e.MockDate >= StartDate);

            if (EndDate.HasValue)
                lamda.And(e => e.MockDate <= EndDate);

            return lamda;
        }
    }

    public class MockPageDtoValidator : PageDtoValidator<MockPageDto>
    {
        public MockPageDtoValidator() : base()
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
