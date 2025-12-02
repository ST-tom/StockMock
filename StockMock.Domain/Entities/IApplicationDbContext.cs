using Microsoft.EntityFrameworkCore;
using StockMock.Domain.Entities.Accounts;
using StockMock.Domain.Entities.Configs;
using StockMock.Domain.Entities.Mocks;
using StockMock.Domain.Entities.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMock.Domain.Entities
{
    public interface IApplicationDbContext
    {
        public DbSet<Account> Accounts { get; }

        public DbSet<Day> Days { get; }

        public DbSet<AccountStock> AccountStocks { get; }

        public DbSet<Stock> Stocks { get; }

        public DbSet<StockDate> StockDates { get; }

        public DbSet<StockTime> StockTimes { get; }

        public DbSet<Mock> Mocks { get; }

        public DbSet<MockDate> MockDates { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

    }
}
