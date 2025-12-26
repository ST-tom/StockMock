using System.Security.Claims;
using TS.Shared.Extension;
using TS.Shared.User;

namespace StockMock.Api.User
{
    public class CurrentUser : IUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public long Id { get; set; }
        public string? Name { get; set; }

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            Id = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier).ToLong() ?? 0;
            Name = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
        }
    }
}
