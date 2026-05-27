using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockMock.Service.Areas.Stocks.Dtos;
using StockMock.Service.Areas.Stocks.Services;
using TS.Shared.WebApi;

namespace StockMock.Api.Controllers
{
    /// <summary>
    /// 股票行情控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "用户")]
    public class StockDateController(StockDateService stockDateService) : IpControllerBase
    {
        private readonly StockDateService _stockDateService = stockDateService;

        #region 增删改查

        /// <summary>
        /// 添加股票行情
        /// </summary>
        /// <param name="dto">股票行情DTO对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] StockDateDto dto, CancellationToken cancellationToken = default)
        {
            await _stockDateService.AddAsync(dto, cancellationToken);
            return ApiResult.OK();
        }

        /// <summary>
        /// 获取股票行情详情
        /// </summary>
        /// <param name="id">行情ID</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>股票行情DTO</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id, CancellationToken cancellationToken = default)
        {
            var dto = await _stockDateService.GetAsync(id, cancellationToken);
            return ApiResult.OK(dto);
        }

        /// <summary>
        /// 更新股票行情
        /// </summary>
        /// <param name="dto">股票行情DTO对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] StockDateDto dto, CancellationToken cancellationToken = default)
        {
            await _stockDateService.UpdateAsync(dto, cancellationToken);
            return ApiResult.OK();
        }

        /// <summary>
        /// 删除股票行情
        /// </summary>
        /// <param name="idText">行情ID列表（逗号分隔）</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] string idText, CancellationToken cancellationToken = default)
        {
            await _stockDateService.DeleteAsync(idText, cancellationToken);
            return ApiResult.OK();
        }

        #endregion

        #region 分页查询

        /// <summary>
        /// 分页加载股票行情列表
        /// </summary>
        /// <param name="pageDto">分页DTO对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>股票行情分页列表</returns>
        [HttpPost("load")]
        public async Task<IActionResult> Load([FromBody] StockDatePageDto pageDto, CancellationToken cancellationToken = default)
        {
            var pageList = await _stockDateService.LoadAsync(pageDto, cancellationToken);
            return ApiResult.OK(pageList);
        }

        #endregion

        #region 导入数据

        /// <summary>
        /// 导入股票行情数据（Excel）
        /// </summary>
        /// <param name="file">Excel文件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        [HttpPost("import")]
        public async Task<IActionResult> Import(IFormFile file, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                return ApiResult.Err("请提供有效的文件");

            await _stockDateService.ImportAsync(file.OpenReadStream(), file.FileName, cancellationToken);
            return ApiResult.OK();
        }

        #endregion
    }
}
