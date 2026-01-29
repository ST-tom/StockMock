using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
