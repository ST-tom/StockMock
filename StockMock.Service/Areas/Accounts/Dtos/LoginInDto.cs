using FluentValidation;

namespace StockMock.Service.Areas.Accounts.Dtos
{
    public class LoginDto
    {
        public string LoginAccount { get; set; }

        public string Password { get; set; }
    }

    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(v => v.LoginAccount)
                .NotEmpty()
                .WithMessage("登录账号不能为空");
          

            RuleFor(v => v.Password)
                .NotEmpty()
                .WithMessage("密码不能为空");
        }
    }
}
