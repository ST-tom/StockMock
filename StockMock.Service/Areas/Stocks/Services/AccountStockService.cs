using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StockMock.Core.Stocks;
using StockMock.Data;
using StockMock.Service.Areas.Stocks.Dtos;
using StockMock.Service.FluentValidation;
using TS.Shared.Excption;
using TS.Shared.Query;

namespace StockMock.Service.Areas.Stocks.Services
{
    public class AccountStockService(ApplicationDbContext context, IMapper mapper, ILogger<AccountStockService> logger)
        : BaseService<AccountStockService>(context, mapper, logger)
    {
        #region 增删改查

        private static async Task ValidateAsync(AccountStockDto dto, bool isNotOnlyCode, CancellationToken cancellationToken)
        {
            AccountStockDtoValidator validator = new(isNotOnlyCode);
            var validationResult = await validator.ValidateAsync(dto, cancellationToken);

            if (!validationResult.IsValid)
                throw new ApplicationExcption(validationResult.Errors.ToMessage());
        }

        public async Task AddAsync(AccountStockDto dto, CancellationToken cancellationToken = default)
        {
            await ValidateAsync(dto, true, cancellationToken);

            var old = await _context.AccountStocks.FirstOrDefaultAsync(e => e.StockCode == dto.Code, cancellationToken);
            if (old != null)
                throw new ApplicationExcption("该股票已经添加，请勿重复添加");

            await _context.AccountStocks.AddAsync(_mapper.Map<AccountStock>(dto), cancellationToken);

            var oldStock = await _context.Stocks.FirstAsync(e => e.Code == dto.Code, cancellationToken);
            if (oldStock == null)
                await _context.Stocks.AddAsync(_mapper.Map<Stock>(dto), cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DisableAsync(AccountStockDto dto, CancellationToken cancellationToken = default)
        {
            await ValidateAsync(dto, false, cancellationToken);

            var old = await _context.AccountStocks.FirstOrDefaultAsync(e => e.StockCode == dto.Code, cancellationToken) ?? throw new ApplicationExcption("该股票尚未添加");
            if (!old.IsEnabled)
                throw new ApplicationExcption("该股票为禁用状态无需禁用");

            old.IsEnabled = true;
            _context.AccountStocks.Update(old);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(AccountStockDto dto, CancellationToken cancellationToken = default)
        {
            await ValidateAsync(dto, false, cancellationToken);

            var old = await _context.AccountStocks.FirstOrDefaultAsync(e => e.StockCode == dto.Code, cancellationToken) ?? throw new ApplicationExcption("该股票尚未添加");
            _context.AccountStocks.Remove(old);

            await _context.SaveChangesAsync(cancellationToken);
        }

        #endregion

        #region 分页数据

        public async Task<PageList<AccountStock>> LoadAsync(AccountStockPageDto pageDto, CancellationToken cancellationToken = default)
        {
            var validator = new AccountStockPageDtoValidator();
            var validationResult = await validator.ValidateAsync(pageDto, cancellationToken);

            if (!validationResult.IsValid)
                throw new ApplicationExcption(validationResult.Errors.ToMessage());

            var queryable = _context.AccountStocks.Where(pageDto.GetWhereLamda());
            var pageList = await pageDto.LoadAsync(queryable, cancellationToken);

            return pageList;
        }

        #endregion
    }
}
