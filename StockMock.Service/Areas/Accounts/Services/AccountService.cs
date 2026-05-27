using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StockMock.Core;
using StockMock.Core.Accounts;
using StockMock.Data;
using StockMock.Service.Areas.Accounts.Dtos;
using StockMock.Service.FluentValidation;
using System.Linq;
using System.Security.Claims;
using TS.Shared.Excption;
using TS.Shared.Extension;
using TS.Shared.Jwt;
using TS.Shared.Query;
using TS.Shared.Util;

namespace StockMock.Service.Areas.Accounts.Services
{
    public class AccountService(IUser user,JwtManager jwtManager, GlobalCacheManager globalCache, ApplicationDbContext context, IMapper mapper, ILogger<AccountService> logger) : BaseService<AccountService>(context, mapper, logger)
    {
        private readonly IUser _user = user;
        private readonly JwtManager _jwtManager = jwtManager;
        private readonly GlobalCacheManager _globalCache = globalCache;

        #region 登录 登出

        /// <summary>
        /// 获取登录尝试次数缓存key
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        private string GetCacheKeyLogInTryCount(LogInDto dto, string ip) => $"account.login.trycount:{ip}:{dto.LogInAccount}";

        public async Task<(string, string)> LogIn(string ip, LogInDto dto, CancellationToken cancellationToken = default)
        {
            LogInDtoValidator validator = new();
            var validationResult = await validator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
                throw new BizException(validationResult.Errors.ToMessage());

            var key = GetCacheKeyLogInTryCount(dto, ip);
            var tryCount = _globalCache.Get<int>(key);
            if (tryCount >= 5)
                throw new BizException("登录尝试次数过多，请稍后再试");

            _globalCache.Set(key, tryCount + 1);

            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.LoginAccount == dto.LogInAccount && x.Password == EncryptionUtil.ToMD5(dto.Password), cancellationToken) ?? throw new BizException("用户不存在或密码有误");
            
            if (!account.IsEnabled)
                throw new BizException("该账户已被禁用");

            (string token,string refreToken) tokens;
            var referToken = _jwtManager.GetRefreshToken(account.Id);
            if (referToken.IsNullOrWhiteSpace())
                tokens = (_jwtManager.NewToken(account.Id, account.Name, [new Claim(ClaimTypes.Role, account.Role.ToString())]), referToken);
            else
                tokens = _jwtManager.NewTokenAndRefreshToken(account.Id, account.Name, [new Claim(ClaimTypes.Role, account.Role.ToString())]);

            account.LastLoginTime = DateTime.Now;

            _globalCache.Set(key, 0);

            _context.Update(account);
            await _context.SaveChangesAsync(cancellationToken);

            return tokens;
        }

        public async Task<string> Refresh(string refreshToken, CancellationToken cancellationToken = default)
        {
            if (refreshToken.IsNullOrEmpty())
                return string.Empty;

            if (!_jwtManager.CheckRefreshToken(_user.Id, refreshToken))
                return string.Empty;
            
            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == _user.Id, cancellationToken);
            if (account == null)
                return string.Empty;

            if (!account.IsEnabled)
                throw new BizException("该账户已被禁用");

            return _jwtManager.NewToken(account.Id, account.Name, [new Claim(ClaimTypes.Role, account.Role.ToString())]);
        }

        /// <summary>
        /// 移除刷新令牌
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>s
        public void RemoveToken() => _jwtManager.RemoveRefreshToken(_user.Id);

        #endregion

        #region 账户管理

        private const string defaultPassword = "123456";

        private async Task ValidateAccountDto(AccountDto dto, bool isUpdate, CancellationToken cancellationToken)
        {
            var validator = new AccountDtoValidator(isUpdate);
            var validationResult = await validator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
                throw new BizException(validationResult.Errors.ToMessage());
        }

        /// <summary>
        /// 新增账户（仅管理员）
        /// </summary>
        public async Task<long> AddAsync(AccountDto dto, CancellationToken cancellationToken = default)
        {
            await ValidateAccountDto(dto, false, cancellationToken);

            var exists = await _context.Accounts.AnyAsync(x => x.LoginAccount == dto.LoginAccount, cancellationToken);
            if (exists)
                throw new BizException("该登录账号已存在，请使用其他账号");

            var account = new Account
            {
                LoginAccount = dto.LoginAccount,
                Password = EncryptionUtil.ToMD5(dto.Password!),
                Name = dto.Name,
                Role = AccountRole.用户,
                IsEnabled = true
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync(cancellationToken);

            return account.Id;
        }

        /// <summary>
        /// 修改自己的账户信息
        /// </summary>
        public async Task UpdateAccountAsync(AccountDto dto, CancellationToken cancellationToken = default)
        {
            await ValidateAccountDto(dto, true, cancellationToken);

            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == _user.Id, cancellationToken)
                ?? throw new BizException("账户不存在");

            account.Name = dto.Name;
            account.Password = EncryptionUtil.ToMD5(dto.Password!);

            _context.Accounts.Update(account);
            await _context.SaveChangesAsync(cancellationToken);

            _jwtManager.RemoveRefreshToken(account.Id);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        public async Task ResetPasswordAsync(AccountDto dto, CancellationToken cancellationToken = default)
        {
            await ValidateAccountDto(dto, false, cancellationToken);

            if (dto.Id != _user.Id)
            {
                var user = await _context.Accounts.FirstOrDefaultAsync(e => e.Id == _user.Id, cancellationToken) ?? throw new BizException("当前登录用户异常");
                if(user.Role != AccountRole.管理员)
                    throw new BizException("当前用户无此操作权限");
            }

            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken)
                ?? throw new BizException("该账户不存在");

            account.Password = EncryptionUtil.ToMD5(defaultPassword);

            _context.Accounts.Update(account);
            await _context.SaveChangesAsync(cancellationToken);

            _jwtManager.RemoveRefreshToken(account.Id);
        }

        /// <summary>
        /// 启用/禁用账户（仅管理员）
        /// </summary>
        public async Task SetEnabledAsync(long accountId, bool isEnabled, CancellationToken cancellationToken = default)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == accountId, cancellationToken)
                ?? throw new BizException("该账户不存在");

            account.IsEnabled = isEnabled;

            _context.Accounts.Update(account);
            await _context.SaveChangesAsync(cancellationToken);

            if (!isEnabled)
                _jwtManager.RemoveRefreshToken(account.Id);
        }

        /// <summary>
        /// 删除账户
        /// </summary>
        public async Task DeleteAsync(long accountId, CancellationToken cancellationToken = default)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == accountId, cancellationToken)
                ?? throw new BizException("该账户不存在");

            if (account.IsEnabled)
                throw new BizException("只能删除已禁用的账户");

            var mockDatas = _context.Mocks.Where(x => x.AccountId == accountId);
            _context.Mocks.RemoveRange(mockDatas);

            var mockDates = _context.MockDates.Where(x => x.AccountId == accountId);
            _context.MockDates.RemoveRange(mockDates);

            var stockData = _context.AccountStocks.Where(x => x.AccountId == accountId);
            _context.AccountStocks.RemoveRange(stockData);

            _context.Accounts.Remove(account);

            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// 分页查询账户列表
        /// </summary>
        public async Task<PageList<Account>> LoadAsync(AccountPageDto pageDto, CancellationToken cancellationToken = default)
        {
            var validator = new AccountPageDtoValidator();
            var validationResult = await validator.ValidateAsync(pageDto, cancellationToken);
            if (!validationResult.IsValid)
                throw new BizException(validationResult.Errors.ToMessage());

            var query = _context.Accounts.AsQueryable().Where(pageDto.GetWhereLamda());
            var queryable = _context.Accounts.Where(pageDto.GetWhereLamda());
            var pageList = await pageDto.LoadAsync(queryable, cancellationToken);

            return pageList;
        }

        #endregion
    }
}
