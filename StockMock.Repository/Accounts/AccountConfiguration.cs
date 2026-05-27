using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockMock.Core.Accounts;
using TS.Shared.Extension;
using TS.Shared.Util;

namespace StockMock.Data.Accounts
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
                IsEnabled = true,
            });

            //用于添加Fluent Api配置

            // 配置索引
            entitys.HasIndex(x => x.IsEnabled);
        }
    }
}
