using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using StockMock.Domain.Entities;
using StockMock.Domain.Entities.Stocks;
using StockMock.Domain.Entities.Mocks;
using StockMock.Domain.Entities.Accounts;
using StockMock.Domain.Entities.Configs;

namespace StockMock.Infrastructure.Database
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Account>Accounts => Set<Account>();

        public DbSet<Day>Days => Set<Day>();

        public DbSet<AccountStock> AccountStocks => Set<AccountStock>();

        public DbSet<Stock> Stocks => Set<Stock>();

        public DbSet<StockDate> StockDates => Set<StockDate>();

        public DbSet<StockTime> StockTimes => Set<StockTime>();

        public DbSet<Mock> Mocks => Set<Mock>();

        public DbSet<MockDate> MockDates => Set<MockDate>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder modelBuilder)
        {
            //移除EFCore默认约束，EF迁移时自动添加s的复数形式
            modelBuilder.Conventions.Remove(typeof(TableNameFromDbSetConvention));
        }
    }
}
