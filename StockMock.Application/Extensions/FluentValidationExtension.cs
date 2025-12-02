using FluentValidation;

namespace StockMock.Application.Extensions
{
    public static class FluentValidationExtension
    {
        public static IRuleBuilderOptions<T, long?> MustId<T>(
            this IRuleBuilder<T, long?> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Id不合法");
        }

        /// <summary>
        /// 验证日期是否在对应日期范围内，默认2000年1月1日至2100年12月31日
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="ruleBuilder">验证规则构建器</param>
        /// <returns>验证规则选项（支持链式调用）</returns>
        public static IRuleBuilderOptions<T, DateOnly> MustDateRange<T>(
            this IRuleBuilder<T, DateOnly> ruleBuilder, DateOnly minDate = default, DateOnly maxDate = default)
        {
            // 定义日期范围边界
            if (minDate == default)
                minDate = new DateOnly(2000, 1, 1);

            if (maxDate == default)
                maxDate = new DateOnly(2100, 12, 31);

            // 封装原有验证逻辑
            return ruleBuilder
                .GreaterThanOrEqualTo(minDate)
                .WithMessage($"日期不能低于{minDate:yyyy年}")
                .LessThanOrEqualTo(maxDate)
                .WithMessage($"日期不能超过{maxDate:yyyy年}");
        }

        /// <summary>
        /// 验证日期是否在对应日期范围内，默认2000年1月1日至2100年12月31日
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="ruleBuilder">验证规则构建器</param>
        /// <returns>验证规则选项（支持链式调用）</returns>
        public static IRuleBuilderOptions<T, DateOnly?> MustDateRange<T>(
            this IRuleBuilder<T, DateOnly?> ruleBuilder, DateOnly minDate = default, DateOnly maxDate = default)
        {
            // 定义日期范围边界
            if (minDate == default)
                minDate = new DateOnly(2000, 1, 1);

            if (maxDate == default)
                maxDate = new DateOnly(2100, 12, 31);

            // 封装原有验证逻辑
            return ruleBuilder
                .GreaterThanOrEqualTo(minDate)
                .WithMessage($"日期不能低于{minDate:yyyy年}")
                .LessThanOrEqualTo(maxDate)
                .WithMessage($"日期不能超过{maxDate:yyyy年}");
        }

        public static IRuleBuilderOptions<T, string> MustStockCode<T>(
            this IRuleBuilder<T, string> ruleBuilder)
        {
            // 封装原有验证逻辑
            return ruleBuilder
                .NotEmpty()
                .WithMessage("股票编码不能为空")
                .MustStockCodeLength();
        }

        public static IRuleBuilderOptions<T, string> MustStockCodeLength<T>(
            this IRuleBuilder<T, string> ruleBuilder)
        {
            // 封装原有验证逻辑
            return ruleBuilder
                .MaximumLength(10)
                .WithMessage("股票代码长度不能超过10");
        }

        public static IRuleBuilderOptions<T, string> MustStockName<T>(
            this IRuleBuilder<T, string> ruleBuilder)
        {
            // 封装原有验证逻辑
            return ruleBuilder
                .NotEmpty()
                .WithMessage("股票名称不能为空")
                .MustStockNameLength();
        }

        public static IRuleBuilderOptions<T, string> MustStockNameLength<T>(
            this IRuleBuilder<T, string> ruleBuilder)
        {
            // 封装原有验证逻辑
            return ruleBuilder
                .MaximumLength(100)
                .WithMessage("股票名称长度不能超过100个字符");
        }

        public static IRuleBuilderOptions<T, decimal?> MustPriceVariationRange<T>(
            this IRuleBuilder<T, decimal?> ruleBuilder)
        {
            // 封装原有验证逻辑
            return ruleBuilder
                .GreaterThanOrEqualTo(-20)
                .WithMessage("跌幅不能低于-20%")
                .LessThanOrEqualTo(20)
                .WithMessage("涨幅不能超过20%");
        }
    }
}
