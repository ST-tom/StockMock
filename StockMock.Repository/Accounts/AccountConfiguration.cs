using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockMock.Core.Accounts;
using TS.Shared.Extension;
using TS.Shared.Util;

namespace StockMock.Repository.Accounts
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> entitys)
        {
            entitys.HasData(new Account()
            {
                LoginAccount = "admin",
                Password = EncryptionUtil.ToMD5("123456"),
                Name = AccountRole.Admin.GetDescription(),
                Role = AccountRole.Admin,
            });

            //用于添加Fluent Api配置

        }
    }
}
