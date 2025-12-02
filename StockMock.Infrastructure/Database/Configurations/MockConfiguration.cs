using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StockMock.Domain.Entities.Mocks;

namespace StockMock.Infrastructure.Database.Configurations
{
    public class MockConfiguration : IEntityTypeConfiguration<Mock>
    {
        public void Configure(EntityTypeBuilder<Mock> entitys)
        {
            //用于添加Fluent Api配置
            entitys.Property(e => e.Status)
                .HasConversion<MockStatusConverter>();
        }
    }

    public class MockStatusConverter : ValueConverter<MockStatus, string>
    {
        public MockStatusConverter(): base(status => status.Key, value => MockStatus.FromKey(value))
        {
        }
    }
}
