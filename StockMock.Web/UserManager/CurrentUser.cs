using StockMock.Application.Extensions;
using StockMock.Domain.Users;
using System.Security.Claims;

namespace StockMock.Web.UserManager
{
    public class CurrentUser : IUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public int Id { get; set; }
        public string? Name { get; set; }

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            if(int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
                Id = id;

            Name = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
        }
    }
}
