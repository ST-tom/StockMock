using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockMock.Service.Areas.Stocks.Dtos;
using StockMock.Service.Areas.Stocks.Services;
using TS.Shared.WebApi;

namespace StockMock.Api.Controllers
{
    /// <summary>
    /// 用户股票控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "用户")]
    public class AccountStockController(AccountStockService accountStockService) : IpControllerBase
    {
        private readonly AccountStockService _accountStockService = accountStockService;

        #region 增删改查

        /// <summary>
        /// 添加用户股票
        /// </summary>
        /// <param name="dto">用户股票DTO对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AccountStockDto dto, CancellationToken cancellationToken = default)
        {
            await _accountStockService.AddAsync(dto, cancellationToken);
            return ApiResult.OK();
        }

        /// <summary>
        /// 禁用用户股票
        /// </summary>
        /// <param name="dto">用户股票DTO对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        [HttpPost("disable")]
        public async Task<IActionResult> Disable([FromBody] AccountStockDto dto, CancellationToken cancellationToken = default)
        {
            await _accountStockService.DisableAsync(dto, cancellationToken);
            return ApiResult.OK();
        }

        /// <summary>
        /// 删除用户股票
        /// </summary>
        /// <param name="dto">用户股票DTO对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] AccountStockDto dto, CancellationToken cancellationToken = default)
        {
            await _accountStockService.DeleteAsync(dto, cancellationToken);
            return ApiResult.OK();
        }

        #endregion

        #region 分页查询

        /// <summary>
        /// 分页加载用户股票列表
        /// </summary>
        /// <param name="pageDto">分页DTO对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>用户股票分页列表</returns>
        [HttpPost("load")]
        public async Task<IActionResult> Load([FromBody] AccountStockPageDto pageDto, CancellationToken cancellationToken = default)
        {
            var pageList = await _accountStockService.LoadAsync(pageDto, cancellationToken);
            return ApiResult.OK(pageList);
        }

        #endregion
    }
}
