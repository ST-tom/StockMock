using LinqKit;
using StockMock.Core.Accounts;
using StockMock.Service.FluentValidation;
using System.Linq.Expressions;
using TS.Shared.Extension;
using TS.Shared.Query;

namespace StockMock.Service.Areas.Accounts.Dtos
{
    public class AccountPageDto : PageDto
    {
        public string? LoginAccount { get; set; }

        public string? Name { get; set; }

        public AccountRole? Role { get; set; }

        public bool? IsEnabled { get; set; }

        public override Expression<Func<Account, bool>> GetWhereLamda()
        {
            var lamda = PredicateBuilder.New<Account>(true);

            if (LoginAccount.IsNotNullOrEmpty())
                lamda = lamda.And(x => x.LoginAccount!.Contains(LoginAccount!));

            if (Name.IsNotNullOrEmpty())
                lamda = lamda.And(x => x.Name!.Contains(Name!));

            if (Role.HasValue)
                lamda = lamda.And(x => x.Role == Role.Value);

            if (IsEnabled.HasValue)
                lamda = lamda.And(x => x.IsEnabled == IsEnabled.Value);

            return lamda;
        }
    }

    public class AccountPageDtoValidator : PageDtoValidator<AccountPageDto>
    {
    }
}
