using System.Text.Json.Serialization;
using TS.Shared.Json;
using TS.Shared.Util;

namespace TS.Shared.Jwt
{
    public class JwtRefreshToken
    {
        /// <summary>
        /// Refresh Token 字符串
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// 关联用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpiredTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        public static JwtRefreshToken New(long userId, TimeSpan expireTimeSpan) =>
            new()
            {
                UserId = userId,
                CreatedTime = DateTime.Now,
                ExpiredTime = DateTime.Now.AddTicks(expireTimeSpan.Ticks),
                Token = RandomNumberUitl.New(),
            };
    }
}
