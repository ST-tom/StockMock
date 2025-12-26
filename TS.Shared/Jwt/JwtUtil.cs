using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TS.Shared.User;

namespace TS.Shared.Jwt
{
    public class JwtUtil(IConfiguration config, IJwtRefreshUtil jwtRefreshUtil)
    {
        protected IConfigurationSection _jwtSetting = config.GetSection("JwtSettings");

        protected IJwtRefreshUtil _refreshUtil = jwtRefreshUtil;

        public (string jwtToken, string refreshToken) CreateToken(IUser user, IEnumerable<string> roles)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting["SecretKey"]!));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Name ?? string.Empty),
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // 生成 Access Token
            var accessToken = new JwtSecurityToken(
                issuer: _jwtSetting["Issuer"],
                audience: _jwtSetting["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSetting["AccessExpiresMinutes"])),
                signingCredentials: credentials
            );

            var accessTokenStr = new JwtSecurityTokenHandler().WriteToken(accessToken);

            var refreshToken = _refreshUtil.CreateRefreshToken(user.Id);
            _refreshUtil.SaveAsync(refreshToken);

            return (accessTokenStr, refreshToken.Token);
        }
    }
}
