using StockMock.Domain.Common;

namespace StockMock.Domain.Entities.Accounts
{
    public class Account : BaseAuditEntity
    {
        public string? LoginAccount { get; set; }

        public string? Password { get; set; }

        public string? Name { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public AccountRole Role { get; set; } = AccountRole.用户;
    }

    public class AccountRole : StringEnumeration
    {
        protected AccountRole(string key, string name) : base(key, name)
        {
        }

        public static AccountRole 管理员 = new AccountRole("Admin", "管理员");
        public static AccountRole 用户 = new AccountRole("User", "用户");

        public static IEnumerable<AccountRole> GetAll()
        {
            return GetAll<AccountRole>();
        }

        public static AccountRole FromName(string roleString)
        {
            return FromName<AccountRole>(roleString);
        }

        public static AccountRole FromKey(string key)
        {
            return FromKey<AccountRole>(key);
        }
    }
}
