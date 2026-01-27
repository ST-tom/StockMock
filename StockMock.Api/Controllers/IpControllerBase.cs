using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace StockMock.Api.Controllers
{
    public class IpControllerBase : ControllerBase
    {
        /// <summary>
        /// 封装的通用获取客户端IP方法（核心逻辑）
        /// </summary>
        protected string GetIpAddress()
        {
            // 从X-Forwarded-For获取（反向代理场景）
            var forwardedForHeader = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedForHeader))
            {
                // X-Forwarded-For格式可能是：客户端IP, 代理IP1, 代理IP2，取第一个即可
                var ipList = forwardedForHeader.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (ipList.Length > 0 && IPAddress.TryParse(ipList[0].Trim(), out var forwardedIp))
                {
                    return FormatIpAddress(forwardedIp);
                }
            }

            // 其次从X-Real-IP获取（部分代理如Nginx常用）
            var realIpHeader = Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIpHeader) && IPAddress.TryParse(realIpHeader.Trim(), out var realIp))
            {
                return FormatIpAddress(realIp);
            }

            // 从Connection获取（无代理场景）
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                return FormatIpAddress(HttpContext.Connection.RemoteIpAddress);
            }

            return string.Empty;
        }

        /// <summary>
        /// 格式化IP地址（处理IPv6的::1转为127.0.0.1，统一IP格式）
        /// </summary>
        protected string FormatIpAddress(IPAddress ipAddress)
        {
            // IPv6本地回环地址(::1)转为IPv4的127.0.0.1，方便统一处理
            if (ipAddress.IsIPv6LinkLocal || ipAddress.Equals(IPAddress.IPv6Loopback))
            {
                return "127.0.0.1";
            }

            // 转换为字符串（去掉IPv6的作用域后缀，如%eth0）
            var ipStr = ipAddress.ToString();
            var scopeIndex = ipStr.IndexOf('%');
            return scopeIndex > 0 ? ipStr[..scopeIndex] : ipStr;
        }
    }
}
