using FluentValidation;

namespace StockMock.Service.Areas.Accounts.Dtos
{
    public class LogInDto
    {
        public string LogInAccount { get; set; }

        public string Password { get; set; }
    }

    public class LogInDtoValidator : AbstractValidator<LogInDto>
    {
        public LogInDtoValidator()
        {
            RuleFor(v => v.LogInAccount)
                .NotEmpty()
                .WithMessage("登录账号不能为空");
          

            RuleFor(v => v.Password)
                .NotEmpty()
                .WithMessage("密码不能为空");
        }
    }
}
