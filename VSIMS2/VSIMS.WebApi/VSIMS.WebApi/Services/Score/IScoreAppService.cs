﻿using VSIMS.WebApi.Dto;
using VSIMS.WebApi.Entity;
using VSIMS.WebApi.Utils;

namespace VSIMS.WebApi.Services.Score
{
    public interface IScoreAppService
    {
        public PagedRequest<ScoreDto> GetScores(string studentName,string courseName,int pageNum,int pageSize);

        /// <summary>
        /// 通过id查询成绩信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ScoreEntity GetScore(int id);

        /// <summary>
        /// 新增成绩
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        public int AddScore(ScoreEntity score);

        /// <summary>
        /// 修改成绩
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        public int UpdateScore(ScoreEntity score);

        /// <summary>
        /// 删除成绩
        /// </summary>
        /// <param name="id"></param>
        public int DeleteScore(int id);
    }
}
