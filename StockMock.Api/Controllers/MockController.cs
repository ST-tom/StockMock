using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockMock.Service.Areas.Mocks.Dtos;
using StockMock.Service.Areas.Mocks.Services;
using TS.Shared.WebApi;

namespace StockMock.Api.Controllers
{
    /// <summary>
    /// 股票模拟控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "用户")]
    public class MockController(MockService mockService) : IpControllerBase
    {
        private readonly MockService _mockService = mockService;

        #region 股票模拟

        #region 增删改查

        /// <summary>
        /// 创建股票模拟
        /// </summary>
        /// <param name="dto">模拟DTO对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] MockDto dto, CancellationToken cancellationToken = default)
        {
            await _mockService.AddAsync(dto, cancellationToken);
            return ApiResult.OK();
        }

        /// <summary>
        /// 取消股票模拟
        /// </summary>
        /// <param name="dto">模拟DTO对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel([FromBody] MockDto dto, CancellationToken cancellationToken = default)
        {
            await _mockService.CancelAsync(dto, cancellationToken);
            return ApiResult.OK();
        }

        /// <summary>
        /// 完成股票模拟
        /// </summary>
        /// <param name="dto">模拟DTO对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        [HttpPost("finish")]
        public async Task<IActionResult> Finish([FromBody] MockDto dto, CancellationToken cancellationToken = default)
        {
            await _mockService.FinishAsync(dto, cancellationToken);
            return ApiResult.OK();
        }

        /// <summary>
        /// 重新开始股票模拟
        /// </summary>
        /// <param name="dto">模拟DTO对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        [HttpPost("restart")]
        public async Task<IActionResult> Restart([FromBody] MockDto dto, CancellationToken cancellationToken = default)
        {
            await _mockService.RestartAsync(dto, cancellationToken);
            return ApiResult.OK();
        }

        #endregion

        #region 分页查询

        /// <summary>
        /// 分页加载股票模拟列表
        /// </summary>
        /// <param name="pageDto">分页DTO对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>模拟分页列表</returns>
        [HttpPost("load")]
        public async Task<IActionResult> Load([FromBody] MockPageDto pageDto, CancellationToken cancellationToken = default)
        {
            var pageList = await _mockService.LoadAsync(pageDto, cancellationToken);
            return ApiResult.OK(pageList);
        }

        #endregion

        #region 模拟详情

        /// <summary>
        /// 加载股票模拟详情
        /// </summary>
        /// <param name="dto">模拟DTO对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>模拟详情DTO</returns>
        [HttpPost("detail")]
        public async Task<IActionResult> LoadDetail([FromBody] MockDto dto, CancellationToken cancellationToken = default)
        {
            var infoDto = await _mockService.LoadDetailAsync(dto, cancellationToken);
            return ApiResult.OK(infoDto);
        }

        #endregion

        #endregion

        #region 股票模拟日期

        #region 新增模拟日期

        /// <summary>
        /// 新增股票模拟日期数据
        /// </summary>
        /// <param name="dto">模拟日期DTO对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        [HttpPost("date/add")]
        public async Task<IActionResult> AddMockDate([FromBody] MockDateDto dto, CancellationToken cancellationToken = default)
        {
            await _mockService.AddMockDate(dto, cancellationToken);
            return ApiResult.OK();
        }

        #endregion

        #region 查询模拟日期

        /// <summary>
        /// 加载最新模拟日期数据（最近30条）
        /// </summary>
        /// <param name="dto">模拟DTO对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>模拟日期列表</returns>
        [HttpPost("date/latest")]
        public async Task<IActionResult> LoadLatestMockDates([FromBody] MockDto dto, CancellationToken cancellationToken = default)
        {
            var mockDates = await _mockService.LoadLatestMockDatesAsync(dto, cancellationToken);
            return ApiResult.OK(mockDates);
        }

        /// <summary>
        /// 分页加载模拟日期数据
        /// </summary>
        /// <param name="pageDto">模拟日期分页DTO对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>模拟日期分页列表</returns>
        [HttpPost("date/load")]
        public async Task<IActionResult> LoadMockDates([FromBody] MockDatePageDto pageDto, CancellationToken cancellationToken = default)
        {
            var pageList = await _mockService.LoadMockDateAsync(pageDto, cancellationToken);
            return ApiResult.OK(pageList);
        }

        #endregion

        #endregion
    }
}
