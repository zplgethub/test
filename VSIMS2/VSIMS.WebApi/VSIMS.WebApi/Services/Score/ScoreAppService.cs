using VSIMS.WebApi.Entity;
using VSIMS.WebApi.Data;
using VSIMS.WebApi.Utils;
using VSIMS.WebApi.Dto;

namespace VSIMS.WebApi.Services.Score
{
    public class ScoreAppService : IScoreAppService
    {
        private ImsDataContext _imsDataContext;

        public ScoreAppService(ImsDataContext imsDataContext)
        {
            this._imsDataContext = imsDataContext;
        }

        public int AddScore(ScoreEntity score)
        {
            var entity = this._imsDataContext.Scores.Add(score);
            this._imsDataContext.SaveChanges();
            return 0;
        }

        public int DeleteScore(int id)
        {
            var entity = _imsDataContext.Scores.FirstOrDefault(x => x.Id == id);
            if (entity != null)
            {
                _imsDataContext.Scores.Remove(entity);
                _imsDataContext.SaveChanges();
            }
            return 0;
        }

        public ScoreEntity GetScore(int id)
        {
            var entity = _imsDataContext.Scores.FirstOrDefault(r => r.Id == id);
            return entity;
        }

        /// <summary>
        /// 按条件查询成绩列表
        /// </summary>
        /// <param name="studentName"></param>
        /// <param name="courseName"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PagedRequest<ScoreDto> GetScores(string studentName, string courseName, int pageNum, int pageSize)
        {
            IQueryable<ScoreEntity> scores = null;
            if (!string.IsNullOrEmpty(studentName) && !string.IsNullOrEmpty(courseName))
            {
                var students = this._imsDataContext.Students.Where(r => r.Name.Contains(studentName));
                var courses = this._imsDataContext.Courses.Where(r => r.Name.Contains(courseName));
                scores = this._imsDataContext.Scores.Where(r => students.Select(t => t.Id).Contains(r.StudentId)).Where(r => courses.Select(t => t.Id).Contains(r.CourseId));
            }
            else if (!string.IsNullOrEmpty(studentName))
            {
                var students = this._imsDataContext.Students.Where(r => r.Name.Contains(studentName));
                scores = this._imsDataContext.Scores.Where(r => students.Select(t => t.Id).Contains(r.StudentId));
            }
            else if (!string.IsNullOrEmpty(courseName))
            {
                var courses = this._imsDataContext.Courses.Where(r => r.Name.Contains(courseName));
                scores = this._imsDataContext.Scores.Where(r => courses.Select(t => t.Id).Contains(r.CourseId));
            }
            else {
                scores = _imsDataContext.Scores.Where(r => true).OrderBy(r => r.Id);
            }
            int count = scores.Count();
            List<ScoreEntity> items;
            if (pageSize > 0)
            {
                items = scores.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                items = scores.ToList();
            }
            var dtos = items.Select(r => ScoreEntityToDto(r));
            return new PagedRequest<ScoreDto>()
            {
                count = count,
                items = dtos.ToList(),
            };
        }

        public int UpdateScore(ScoreEntity score)
        {
            _imsDataContext.Scores.Update(score);
            _imsDataContext.SaveChanges();
            return 0;
        }

        private ScoreDto ScoreEntityToDto(ScoreEntity entity) {
            var studentName = string.Empty;
            var courseName = string.Empty;
            var studentId = entity.StudentId;
            var courseId = entity.CourseId;
            
            var student = _imsDataContext.Students.FirstOrDefault(r => r.Id == studentId);
            if (student != null)
            {
                studentName = $"{student.Name}";
            }

            var course = _imsDataContext.Courses.FirstOrDefault(r => r.Id == courseId);
            if (course != null)
            {
                courseName = $"{course.Name}";
            }

            var dto = new ScoreDto()
            {
                Id = entity.Id,
                StudentId = studentId,
                CourseId = courseId,
                StudentName = studentName,
                CourseName = courseName,
                Score=entity.Score,
                CreateTime = entity.CreateTime,
                CreateUser = entity.CreateUser,
                LastEditTime = entity.LastEditTime,
                LastEditUser = entity.LastEditUser,

            };
            return dto;
        }
    }
}
