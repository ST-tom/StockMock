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
    }

    public enum AccountRole 
    {
        [Description("管理员")]
        Admin,
        [Description("用户")]
        User, 
    }
}
