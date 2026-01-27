using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TS.Shared.Extension;

namespace TS.Shared.Jwt
{
    public class JwtManager(IConfiguration config)
    {
        private DateTime _lastScanTime = DateTime.Now;

        private readonly ConcurrentDictionary<string, JwtRefreshToken> _refreshTokenDic = new();

        protected readonly IConfigurationSection _jwtSetting = config.GetSection("JwtSettings");

        /// <summary>
        /// 创建 Access Token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public string NewToken(long id, string? name, params Claim[] claims)
        {
            ScanRefreshTokenDic();

            var secretKey = _jwtSetting.GetValue<string>("SecretKey");
            var issuer = _jwtSetting.GetValue<string>("Issuer");
            var audience = _jwtSetting.GetValue<string>("Audience");
            double? expireMinutes = _jwtSetting.GetValue<double?>("AccessExpiresMinutes");

            ArgumentException.ThrowIfNullOrWhiteSpace(secretKey);
            ArgumentException.ThrowIfNullOrWhiteSpace(issuer);
            ArgumentException.ThrowIfNullOrWhiteSpace(audience);
            if (!expireMinutes.HasValue)
                throw new ArgumentNullException(nameof(expireMinutes));           

            var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), SecurityAlgorithms.HmacSha256);

            if (!claims.HasData())
                claims = [new(JwtRegisteredClaimNames.Sub, id.ToString()), new(JwtRegisteredClaimNames.Name, name ?? string.Empty)];

            // 生成 Access Token
            var accessToken = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expireMinutes.Value),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(accessToken);
        }

        /// <summary>
        /// 创建 Access Token 和 Refresh Token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public (string token, string refreshToken) NewTokenAndRefreshToken(long id, string? name, params Claim[] claims)
        {
            ScanRefreshTokenDic();

            var accessToken = NewToken(id, name, claims);

            var refreshToken = JwtRefreshToken.New(id, TimeSpan.FromDays(_jwtSetting.GetValue<double>("RefreshExpiresDays")));
            _refreshTokenDic.AddOrUpdate(refreshToken.Token, refreshToken, (id, token) => refreshToken);

            return (accessToken, refreshToken.Token);
        }

        /// <summary>
        /// 校验 Refersh Token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claims"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public long CheckRefreshToken(long id, string refreshToken)
        {
            ScanRefreshTokenDic();

            if (_refreshTokenDic.TryGetValue(refreshToken, out var refreshTokenInfo) && refreshTokenInfo.UserId == id && refreshTokenInfo.ExpiredTime > DateTime.Now)
                return refreshTokenInfo.UserId;

            return 0;
        }

        public void RemoveRefreshToken(string refreshToken)
        {
            _refreshTokenDic.TryRemove(refreshToken, out _);
        }

        private void ScanRefreshTokenDic()
        {
            if (DateTime.Now - _lastScanTime < TimeSpan.FromMinutes(5))
                return;

            _lastScanTime = DateTime.Now;

            var removeTokens = new List<string>();

            foreach (var refreshToken in _refreshTokenDic)
            {
                if (refreshToken.Value.ExpiredTime < DateTime.Now)
                    removeTokens.Add(refreshToken.Key);
            }

            foreach (var removeToken in removeTokens)
            {
                _refreshTokenDic.TryRemove(removeToken, out _);
            }
        }
    }
}
