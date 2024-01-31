

using VSIMS.WebApi.Data;
using VSIMS.WebApi.Entity;
using VSIMS.WebApi.Utils;

namespace VSIMS.WebApi.Services.Course
{
    public class CourseAppService : ICourseAppService
    {
        private ImsDataContext _imsDataContext;

        public CourseAppService(ImsDataContext imsDataContext)
        {
            this._imsDataContext = imsDataContext;
        }

        public int AddCourse(CourseEntity course)
        {
            var entry = _imsDataContext.Courses.Add(course);
            _imsDataContext.SaveChanges();
            return 0;
        }

        public int DeleteCourse(int id)
        {
            var entity = _imsDataContext.Courses.FirstOrDefault(x => x.Id == id);
            if (entity != null)
            {
                _imsDataContext.Courses.Remove(entity);
                _imsDataContext.SaveChanges();
            }
            return 0;
        }

        /// <summary>
        /// 根据ID获取单个课程
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CourseEntity GetCourse(int id)
        {
            var entity = _imsDataContext.Courses.FirstOrDefault(r => r.Id == id);
            return entity;
        }

        /// <summary>
        /// 获取课程列表
        /// </summary>
        /// <param name="courseName"></param>
        /// <param name="teacher"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PagedRequest<CourseEntity> GetCourses(string courseName, string teacher, int pageNum, int pageSize)
        {
            IQueryable<CourseEntity> courses = null;
            if (!string.IsNullOrEmpty(courseName) && !string.IsNullOrEmpty(teacher))
            {
                courses = _imsDataContext.Courses.Where(r => r.Name.Contains(courseName) && r.Teacher.Contains(teacher)).OrderBy(r => r.Id);
            }
            else if (!string.IsNullOrEmpty(courseName))
            {
                courses = _imsDataContext.Courses.Where(r => r.Name.Contains(courseName)).OrderBy(r => r.Id);
            }
            else if (!string.IsNullOrEmpty(teacher))
            {
                courses = _imsDataContext.Courses.Where(r => r.Teacher.Contains(teacher)).OrderBy(r => r.Id);
            }
            else
            {
                courses = _imsDataContext.Courses.Where(r => true).OrderBy(r => r.Id);
            }
            int count = courses.Count();
            List<CourseEntity> items;
            if (pageSize > 0)
            {
                items = courses.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                items = courses.ToList();
            }
            return new PagedRequest<CourseEntity>()
            {
                count = count,
                items = items
            };
        }

        public int UpdateCourse(CourseEntity course)
        {
            _imsDataContext.Courses.Update(course);
            _imsDataContext.SaveChanges();
            return 0;
        }
    }
}
