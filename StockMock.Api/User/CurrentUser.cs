using StockMock.Core;
using StockMock.Core.Accounts;
using System.Security.Claims;
using TS.Shared.Extension;

namespace StockMock.Api.User
{
    public class CurrentUser : IUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public long Id { get; set; }

        public string? Name { get; set; }

        public AccountRole Role { get; set; }

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            Id = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier).ToLong() ?? 0;
            Name = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
            Role = Enum.Parse<AccountRole>(_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.ToString()!);
        }
    }
}
