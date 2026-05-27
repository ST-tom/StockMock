using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockMock.Service.Areas.Configs.Dtos;
using StockMock.Service.Areas.Configs.Services;
using TS.Shared.WebApi;

namespace StockMock.Api.Controllers
{
    /// <summary>
    /// 交易日配置控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "管理员")]
    public class DayController(DayService dayService) : IpControllerBase
    {
        private readonly DayService _dayService = dayService;

        #region 增删改查

        /// <summary>
        /// 添加交易日
        /// </summary>
        /// <param name="dto">交易日DTO对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] DayDto dto, CancellationToken cancellationToken = default)
        {
            await _dayService.AddAsync(dto, cancellationToken);
            return ApiResult.OK();
        }

        /// <summary>
        /// 获取交易日详情
        /// </summary>
        /// <param name="id">交易日ID</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>交易日DTO</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id, CancellationToken cancellationToken = default)
        {
            var dto = await _dayService.GetAsync(id, cancellationToken);
            return ApiResult.OK(dto);
        }

        /// <summary>
        /// 更新交易日
        /// </summary>
        /// <param name="dto">交易日DTO对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] DayDto dto, CancellationToken cancellationToken = default)
        {
            await _dayService.UpdateAsync(dto, cancellationToken);
            return ApiResult.OK();
        }

        #endregion

        #region 批量生成

        /// <summary>
        /// 批量生成当前日期的全年数据
        /// </summary>
        /// <param name="dto">交易日DTO对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        [HttpPost("build-year")]
        public async Task<IActionResult> BuildYear([FromBody] DayDto dto, CancellationToken cancellationToken = default)
        {
            await _dayService.BuildYearDaysAsync(dto, cancellationToken);
            return ApiResult.OK();
        }

        #endregion

        #region 分页查询

        /// <summary>
        /// 分页加载交易日列表
        /// </summary>
        /// <param name="pageDto">分页DTO对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>交易日分页列表</returns>
        [HttpPost("load")]
        public async Task<IActionResult> Load([FromBody] DayPageDto pageDto, CancellationToken cancellationToken = default)
        {
            var pageList = await _dayService.LoadAsync(pageDto, cancellationToken);
            return ApiResult.OK(pageList);
        }

        #endregion

        #region 工作日查询

        /// <summary>
        /// 判断是否是工作日
        /// </summary>
        /// <param name="date">日期（格式：yyyy-MM-dd）</param>
        /// <returns>是否是工作日</returns>
        [HttpGet("is-workday")]
        public IActionResult IsWorkDay(DateOnly date)
        {
            var isWorkDay = _dayService.IsWorkDay(date);
            return ApiResult.OK(isWorkDay);
        }

        /// <summary>
        /// 获取指定日期的前一个工作日
        /// </summary>
        /// <param name="date">日期（格式：yyyy-MM-dd），默认为当天</param>
        /// <returns>前一个工作日</returns>
        [HttpGet("pre-workday")]
        public IActionResult GetPreWorkDay(DateOnly? date = null)
        {
            var preWorkDay = _dayService.GetPreWorkDay(date);
            return ApiResult.OK(preWorkDay);
        }

        /// <summary>
        /// 获取最近的工作日列表（默认最近30个）
        /// </summary>
        /// <param name="date">日期（格式：yyyy-MM-dd），默认为当天</param>
        /// <param name="dayRange">日期范围，默认30天</param>
        /// <returns>工作日列表</returns>
        [HttpGet("pre-workdays")]
        public IActionResult GetPreWorkDays(DateOnly? date = null, int dayRange = 30)
        {
            var preWorkDays = _dayService.GetPreWorkDays(date, dayRange);
            return ApiResult.OK(preWorkDays);
        }

        /// <summary>
        /// 获取指定日期的下一个工作日
        /// </summary>
        /// <param name="date">日期（格式：yyyy-MM-dd），默认为当天</param>
        /// <returns>下一个工作日</returns>
        [HttpGet("next-workday")]
        public IActionResult GetNextWorkDay(DateOnly? date = null)
        {
            var nextWorkDay = _dayService.GetNextWorkDay(date);
            return ApiResult.OK(nextWorkDay);
        }

        #endregion
    }
}
