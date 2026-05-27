using FluentValidation;
using StockMock.Core.Accounts;

namespace StockMock.Service.Areas.Accounts.Dtos
{
    public class AccountDto
    {
        public long Id { get; set; }

        public string? LoginAccount { get; set; }

        public string? Password { get; set; }

        public string? Name { get; set; }
    }

    public class AccountDtoValidator : AbstractValidator<AccountDto>
    {
        public AccountDtoValidator(bool isUpdate = false)
        {
            if (!isUpdate)
            {
                RuleFor(x => x.LoginAccount)
                    .NotEmpty().WithMessage("登录账号不能为空")
                    .MinimumLength(3).WithMessage("登录账号长度不能少于3个字符")
                    .MaximumLength(50).WithMessage("登录账号长度不能超过50个字符")
                    .Matches(@"^[a-zA-Z0-9_@.-]+$").WithMessage("登录账号只能包含字母、数字、下划线、@、点、连字符");
            }
            else
            {
                RuleFor(x => x.Id)
                    .GreaterThan(0)
                    .WithMessage("当前用户id不合法");
            }

            // 密码验证
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("密码不能为空")
                .MinimumLength(6).WithMessage("密码长度不能少于6个字符")
                .MaximumLength(50).WithMessage("密码长度不能超过50个字符");

            // 姓名验证
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("姓名不能为空")
                .MaximumLength(50).WithMessage("姓名长度不能超过50个字符");
        }
    }
}
