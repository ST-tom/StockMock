using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockMock.Domain.Entities.Accounts;
using StockMock.Infrastructure.Utils;

namespace StockMock.Infrastructure.Database.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> entitys)
        {
            entitys.HasData(new Account()
            {
                LoginAccount = "admin",
                Password = EncryptionUtil.ToMD5("123456"),
                Name = "管理员",
                Role = AccountRole.管理员,
            });

            //用于添加Fluent Api配置

        }
    }
}
