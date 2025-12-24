using FluentValidation.Results;
using StockMock.Infrastructure.Extensions;

namespace StockMock.Application.Extensions
{
    public static class ValidationFailureExtension
    {
        public static string ToMessage(this IEnumerable<ValidationFailure> failures)
        {
            if(failures == null || !failures.Any())
                throw new ArgumentNullException(nameof(failures));

            return failures.Select(e => e.ErrorMessage).ToJoinString();
        }
    }
}
