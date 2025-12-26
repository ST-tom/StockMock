using System;
using System.Collections.Generic;
using System.Text;

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
        public DateTime Expires { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
