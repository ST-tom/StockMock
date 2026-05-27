using StockMock.Core.Accounts;

namespace StockMock.Core
{
    public interface IUser
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public AccountRole Role { get; set; }
    }
}
