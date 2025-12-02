using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StockMock.Application.Areas.AccountStocks.Dtos;
using StockMock.Application.Areas.Stocks.Dtos;
using StockMock.Application.Base;
using StockMock.Application.Extensions;
using StockMock.Domain.Common;
using StockMock.Domain.Common.Caches;
using StockMock.Domain.Entities.Stocks;
using StockMock.Infrastructure.Database;


namespace StockMock.Application.Areas.Stocks.Services
{
    public class AccountStockService : BaseService
    {
        public AccountStockService(ApplicationDbContext context, IMapper mapper, CancellationToken cancellationToken, LocalCacheManager localCache) : base(context, mapper, cancellationToken, localCache)
        {

        }

        private async Task ValidateAsync(AccountStockDto dto, bool isValidateName = false)
        {
            AccountStockDtoValidator validator = new AccountStockDtoValidator(true);
            var validationResult = await validator.ValidateAsync(dto, _cancellationToken);

            if (!validationResult.IsValid)
                throw new BusinessExcption(validationResult.Errors.ToMessage());
        }

        public async Task AddAsync(AccountStockDto dto)
        {
            await ValidateAsync(dto, true);

            var old = await _context.AccountStocks.FirstOrDefaultAsync(e => e.StockCode == dto.Code, _cancellationToken);
            if (old != null)
                throw new BusinessExcption("该股票已经添加，请勿重复添加");

            await _context.AccountStocks.AddAsync(_mapper.Map<AccountStock>(dto), _cancellationToken);

            var oldStock = await _context.Stocks.FirstAsync(e => e.Code == dto.Code, _cancellationToken);
            if (oldStock == null)
                await _context.Stocks.AddAsync(_mapper.Map<Stock>(dto), _cancellationToken);

            await _context.SaveChangesAsync(_cancellationToken);
        }

        public async Task DisableAsync(AccountStockDto dto)
        {
            await ValidateAsync(dto);

            var old = await _context.AccountStocks.FirstOrDefaultAsync(e => e.StockCode == dto.Code, _cancellationToken);
            if (old == null)
                throw new BusinessExcption("该股票尚未添加");

            if (old.IsEnabled == false)
                throw new BusinessExcption("该股票为禁用状态无需禁用");

            old.IsEnabled = true;
            _context.AccountStocks.Update(old);

            await _context.SaveChangesAsync(_cancellationToken);
        }

        public async Task DeleteAsync(AccountStockDto dto)
        {
            await ValidateAsync(dto);

            var old = await _context.AccountStocks.FirstOrDefaultAsync(e => e.StockCode == dto.Code, _cancellationToken);
            if (old == null)
                throw new BusinessExcption("该股票尚未添加");

            _context.AccountStocks.Remove(old);

            await _context.SaveChangesAsync(_cancellationToken);
        }

        public async Task<PageList<AccountStock>> LoadAsync(AccountStockPageDto pageDto)
        {
            var validator = new AccountStockPageDtoValidator();
            var validationResult = await validator.ValidateAsync(pageDto, _cancellationToken);

            if (!validationResult.IsValid)
                throw new BusinessExcption(validationResult.Errors.ToMessage());

            var queryable = _context.AccountStocks.Where(pageDto.GetWhereLamda());
            var pageList = await pageDto.LoadAsync(queryable, _cancellationToken);

            return pageList;
        }
    }
}
