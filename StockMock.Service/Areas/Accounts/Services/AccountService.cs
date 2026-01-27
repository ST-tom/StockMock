using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StockMock.Core.Accounts;
using StockMock.Data;
using StockMock.Service.Areas.Accounts.Dtos;
using StockMock.Service.FluentValidation;
using System.Security.Claims;
using TS.Shared.Excption;
using TS.Shared.Extension;
using TS.Shared.Jwt;
using TS.Shared.User;
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
        public string GetCacheKeyLoginTryCount(LoginDto dto, string ip) => $"account.login.trycount:{ip}:{dto.LoginAccount}";

        /// <summary>
        /// 验证登录模型
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> CheckLoginDto(LoginDto dto, CancellationToken cancellationToken = default)
        {
            LoginDtoValidator validator = new();
            var validationResult = await validator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
                return validationResult.Errors.ToMessage();

            return string.Empty;
        }

        /// <summary>
        /// 校验登录尝试次数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public bool CheckRetryCount(string key)
        {
            var tryCount = _globalCache.Get<int>(key);
            if (tryCount >= 5)
                return false;

            _globalCache.Set(key, tryCount + 1);

            return true;
        }

        /// <summary>
        /// 根据账号密码获取用户
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Account?> GetByAccountAndPassword(LoginDto dto, CancellationToken cancellationToken = default) => await _context.Accounts.FirstOrDefaultAsync(x => x.LoginAccount == dto.LoginAccount && x.Password == EncryptionUtil.ToMD5(dto.Password), cancellationToken);

        public async Task<(string, string)> CreatTokens(Account account, string key, CancellationToken cancellationToken = default)
        {
            if (account == null)
                return (string.Empty, string.Empty);

            var (token, refreToken) = _jwtManager.NewTokenAndRefreshToken(account.Id, account.Name, [new Claim(ClaimTypes.Role, account.Role.GetDescription())]);

            account.LastLoginTime = DateTime.Now;

            _globalCache.Set(key, 0);

            _context.Update(account);
            await _context.SaveChangesAsync(cancellationToken);

            return (token, refreToken);
        }

        public async Task<string> Refresh(string refreshToken, CancellationToken cancellationToken = default)
        {
            if (refreshToken.IsNullOrEmpty())
                return string.Empty;

            var userId = _jwtManager.CheckRefreshToken(_user.Id, refreshToken);
            if(userId == 0)
                return string.Empty;
            
            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
            if (account == null)
                return string.Empty;

            return _jwtManager.NewToken(account.Id, account.Name, [new Claim(ClaimTypes.Role, account.Role.GetDescription())]);
        }

        /// <summary>
        /// 移除刷新令牌
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public void RemoveToken(string refreshToken) => _jwtManager.RemoveRefreshToken(refreshToken);

        #endregion
    }
}
