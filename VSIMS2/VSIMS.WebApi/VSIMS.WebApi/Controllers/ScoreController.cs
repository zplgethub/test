using Microsoft.AspNetCore.Mvc;
using VSIMS.WebApi.Dto;
using VSIMS.WebApi.Entity;
using VSIMS.WebApi.Services.Score;
using VSIMS.WebApi.Utils;

namespace VSIMS.WebApi.Controllers
{
    /// <summary>
    /// 成绩控制器
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ScoreController : ControllerBase
    {
        private readonly ILogger<ScoreController> logger;

        private readonly IScoreAppService scoreAppService;

        public ScoreController(ILogger<ScoreController> logger, IScoreAppService scoreAppService)
        {
            this.logger = logger;
            this.scoreAppService = scoreAppService;
        }

        /// <summary>
        /// 获取成绩信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public PagedRequest<ScoreDto> GetScores(string? studentName, string? courseName, int pageNum, int pageSize)
        {
            return scoreAppService.GetScores(studentName, courseName, pageNum, pageSize);
        }

        /// <summary>
        /// 获取成绩信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ScoreEntity GetScore(int id)
        {
            return scoreAppService.GetScore(id);
        }

        /// <summary>
        /// 新增成绩
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        [HttpPost]
        public int AddScore(ScoreEntity score)
        {
            return scoreAppService.AddScore(score);
        }

        /// <summary>
        /// 修改成绩
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        [HttpPut]
        public int UpdateScore(ScoreEntity score)
        {
            return scoreAppService.UpdateScore(score);
        }

        /// <summary>
        /// 删除成绩
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete]
        public int DeleteScore(int id)
        {
            return scoreAppService.DeleteScore(id);
        }
    }
}
