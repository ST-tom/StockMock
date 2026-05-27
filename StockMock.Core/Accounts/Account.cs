using StockMock.Core;
using System.ComponentModel;

namespace StockMock.Core.Accounts
{
    public class Account : BaseAuditEntity
    {
        public string? LoginAccount { get; set; }

        public string? Password { get; set; }

        public string? Name { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public AccountRole Role { get; set; }

        public bool IsEnabled { get; set; } = true;
    }

    public enum AccountRole
    {
        管理员,
        用户,
    }
}
