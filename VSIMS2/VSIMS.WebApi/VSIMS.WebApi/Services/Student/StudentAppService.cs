using VSIMS.WebApi.Entity;
using VSIMS.WebApi.Data;
using System.Linq.Expressions;
using VSIMS.WebApi.Utils;
using VSIMS.WebApi.Dto;

namespace VSIMS.WebApi.Services.Student
{
    public class StudentAppService : IStudentAppService
    {
        private ImsDataContext _imsDataContext;

        public StudentAppService(ImsDataContext imsDataContext) { 
            this._imsDataContext = imsDataContext;
        }

        /// <summary>
        /// 新增学生
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public int AddStudent(StudentEntity student)
        {
            var entry = _imsDataContext.Students.Add(student);
            _imsDataContext.SaveChanges();
            return 0;
        }

        /// <summary>
        /// 删除学生
        /// </summary>
        /// <param name="id"></param>
        public int DeleteStudent(int id)
        {
            var entity = _imsDataContext.Students.FirstOrDefault(x => x.Id == id);
            if (entity != null)
            {
                _imsDataContext.Students.Remove(entity);
                _imsDataContext.SaveChanges();
            }
            return 0;
        }

        /// <summary>
        /// 查询学生列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pageNum">页数</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns></returns>
        public PagedRequest<StudentDto> GetStudents(string no,string name,int pageNum,int pageSize)
        {
            IQueryable<StudentEntity> students = null;
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(no))
            {
                students = _imsDataContext.Students.Where(r => r.Name.Contains(name) && r.No.Contains(no)).OrderBy(r => r.Id);
            }
            else if (!string.IsNullOrEmpty(name))
            {
                students = _imsDataContext.Students.Where(r => r.Name.Contains(name)).OrderBy(r => r.Id);
            }
            else if (!string.IsNullOrEmpty(no)) {
                students = _imsDataContext.Students.Where(r => r.No.Contains(no)).OrderBy(r => r.Id);
            }
            else
            {
                students = _imsDataContext.Students.Where(r => true).OrderBy(r => r.Id);
            }
            int count = students.Count();
            List<StudentEntity> items;
            if (pageSize > 0)
            {
                items = students.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();
            }
            else {
                items = students.ToList();
            }
            var dtos = items.Select(r => StudentEntityToDto(r));
            return new PagedRequest<StudentDto>()
            {
                count = count,
                items = dtos.ToList(),
            };
        }

        /// <summary>
        /// 查询某一班级的学生列表
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public PagedRequest<StudentEntity> GetStudentsByClasses(int classId)
        {
            IQueryable<StudentEntity> students = _imsDataContext.Students.Where(r => r.ClassesId==classId).OrderBy(r => r.Id);
            int count = students.Count();
            var items = students.ToList();
            return new PagedRequest<StudentEntity>()
            {
                count = count,
                items = items
            };
        }

        /// <summary>
        /// 通过id查询学生信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public StudentEntity GetSudent(int id)
        {
            var entity = _imsDataContext.Students.FirstOrDefault(r => r.Id == id);
            return entity;
        }

        /// <summary>
        /// 修改学生
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public int UpdateStudent(StudentEntity student)
        {
            _imsDataContext.Students.Update(student);
            _imsDataContext.SaveChanges();
            return 0;
        }

        public StudentDto StudentEntityToDto(StudentEntity entity) {
            var classesName = string.Empty;
            var classesId = entity.ClassesId;
            if (classesId != null) { 
                var classes=_imsDataContext.Classes.FirstOrDefault(r=>r.Id == classesId);
                if (classes != null) { 
                classesName= $"{classes.Dept}{classes.Grade}{classes.Name}";
                }
            }
            var dto = new StudentDto()
            {
                Id = entity.Id,
                Name = entity.Name,
                Age = entity.Age,
                ClassesId = entity.ClassesId,
                CreateTime = entity.CreateTime,
                CreateUser = entity.CreateUser,
                LastEditTime = entity.LastEditTime,
                LastEditUser = entity.LastEditUser,
                No = entity.No,
                Sex = entity.Sex,
                ClassesName = classesName
            };
            return dto;
        }
    }
}
