namespace VSIMS.WebApi.Dto
{
    public class ScoreDto
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        ///学生id
        /// </summary>
        public int StudentId { get; set; }

        public string StudentName { get; set; }

        /// <summary>
        /// 课程id
        /// </summary>
        public int CourseId { get; set; }

        public string CourseName { get; set; }

        /// <summary>
        /// 成绩
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 当前登录的账号的ID
        /// </summary>
        public int CreateUser { get; set; }

        /// <summary>
        /// 最后编辑时间
        /// </summary>
        public DateTime LastEditTime { get; set; }

        /// <summary>
        /// 最后修改人
        /// </summary>
        public int LastEditUser { get; set; }
    }
}
