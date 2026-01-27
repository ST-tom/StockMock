using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockMock.Service.Areas.Accounts.Dtos;
using StockMock.Service.Areas.Accounts.Services;
using TS.Shared.Extension;
using TS.Shared.WebApi;

namespace StockMock.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController(AccountService accountService) : IpControllerBase
    {
        private readonly AccountService _accountService = accountService;

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken = default)
        {
            var errmsg = await _accountService.CheckLoginDto(loginDto, cancellationToken);
            if(errmsg.IsNotNullOrEmpty())
                return ApiResult.Err(errmsg);

            var key = _accountService.GetCacheKeyLoginTryCount(loginDto, GetIpAddress());
            if (!_accountService.CheckRetryCount(key))
                return ApiResult.Err("登录失败次数过多，请稍后再试");

            var account = await _accountService.GetByAccountAndPassword(loginDto, cancellationToken);
            if (account == null)
                return ApiResult.Err("用户名或密码错误");
            
            var (token, refreshToken) = await _accountService.CreatTokens(account, key, cancellationToken);
            return ApiResult.OK((token, refreshToken));
        }

        [HttpPost]
        public async Task<IActionResult> LogOut(string accessToken)
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
