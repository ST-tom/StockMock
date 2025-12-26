using System;
using System.Collections.Generic;
using System.Text;

namespace TS.Shared.Jwt
{
    public class JwtSetting
    {
        /// <summary>
        /// 密钥（至少128位，推荐256位）
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// 发行者
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// 受众
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Access Token 过期时间（分钟，建议15-30分钟）
        /// </summary>
        public int AccessTokenExpireMinutes { get; set; } = 15;

        /// <summary>
        /// Refresh Token 过期时间（天，建议7天）
        /// </summary>
        public int RefreshTokenExpireDays { get; set; } = 7;
    }
}
