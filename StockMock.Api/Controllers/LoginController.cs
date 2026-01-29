using Microsoft.AspNetCore.Mvc;
using StockMock.Service.Areas.Accounts.Dtos;
using StockMock.Service.Areas.Accounts.Services;
using TS.Shared.Extension;
using TS.Shared.WebApi;

namespace StockMock.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogInController(AccountService accountService) : IpControllerBase
    {
        private readonly AccountService _accountService = accountService;

        [HttpPost]
        public async Task<IActionResult> LogIn([FromBody] LogInDto loginDto, CancellationToken cancellationToken = default)
        {
            var (token, refreshToken) = await _accountService.LogIn(this.GetIpAddress(), loginDto, cancellationToken);
            return ApiResult.OK((token, refreshToken));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveToken(string accessToken)
        {
            if(accessToken.IsNullOrEmpty())
                return ApiResult.Err("请提供有效的access_token");

            _accountService.RemoveToken(accessToken);

            return ApiResult.OK();
        }

        [HttpPost]
        public async Task<IActionResult> Refresh(string refreshToken, CancellationToken cancellationToken = default)
        {
            if (refreshToken.IsNullOrEmpty())
                return ApiResult.Err("请提供有效的refresh_token");

            var token = await _accountService.Refresh(refreshToken, cancellationToken);
            if(token.IsNullOrEmpty())
                return ApiResult.Err("refresh_token不存在或者已过期，刷新失败");

            return ApiResult.OK(token);
        }
    }
}
