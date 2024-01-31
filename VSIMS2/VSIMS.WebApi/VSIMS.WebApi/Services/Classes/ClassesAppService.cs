using VSIMS.WebApi.Data;
using VSIMS.WebApi.Dto;
using VSIMS.WebApi.Entity;
using VSIMS.WebApi.Utils;

namespace VSIMS.WebApi.Services.Classes
{
    public class ClassesAppService : IClassesAppService
    {
        private ImsDataContext _imsDataContext;

        public ClassesAppService(ImsDataContext imsDataContext)
        {
            this._imsDataContext = imsDataContext;
        }

        /// <summary>
        /// 新增班级
        /// </summary>
        /// <param name="classes"></param>
        /// <returns></returns>
        public int AddClasses(ClassesEntity classes)
        {
            var entry = _imsDataContext.Classes.Add(classes);
            _imsDataContext.SaveChanges();
            return 0;
        }

        /// <summary>
        /// 删除班级
        /// </summary>
        /// <param name="id"></param>
        public int DeleteClasses(int id)
        {
            var entity = _imsDataContext.Classes.FirstOrDefault(x => x.Id == id);
            if (entity != null)
            {
                _imsDataContext.Classes.Remove(entity);
                _imsDataContext.SaveChanges();
            }
            return 0;
        }

        /// <summary>
        /// 查询班级
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ClassesEntity GetClasses(int id)
        {
            var entity = _imsDataContext.Classes.FirstOrDefault(r => r.Id == id);
            return entity;
        }

        public PagedRequest<ClassesDto> GetClassess(string dept, string grade, int pageNum, int pageSize)
        {
            IQueryable<ClassesEntity> classes = null;
            if (!string.IsNullOrEmpty(dept) && !string.IsNullOrEmpty(grade))
            {
                classes = _imsDataContext.Classes.Where(r => r.Dept.Contains(dept) && r.Grade.Contains(grade)).OrderBy(r => r.Id);
            }
            else if (!string.IsNullOrEmpty(dept))
            {
                classes = _imsDataContext.Classes.Where(r => r.Dept.Contains(dept)).OrderBy(r => r.Id);
            }
            else if (!string.IsNullOrEmpty(grade))
            {
                classes = _imsDataContext.Classes.Where(r => r.Grade.Contains(grade)).OrderBy(r => r.Id);
            }
            else
            {
                classes = _imsDataContext.Classes.Where(r => true).OrderBy(r => r.Id);
            }
            int count = classes.Count();
            List<ClassesEntity> items=new List<ClassesEntity>();
            if (pageSize < 1)
            {
                items = classes.ToList();
            }
            else { 
               items = classes.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();
            }
            var dtos = items.Select(r => ClassesEntityToDto(r));
            return new PagedRequest<ClassesDto>()
            {
                count = count,
                items = dtos.ToList()
            };
        }

        /// <summary>
        /// 修改班级
        /// </summary>
        /// <param name="classes"></param>
        /// <returns></returns>
        public int UpdateClasses(ClassesEntity classes)
        {
            _imsDataContext.Classes.Update(classes);
            _imsDataContext.SaveChanges();
            return 0;
        }

        private ClassesDto ClassesEntityToDto(ClassesEntity entity) {
            var monitorName = string.Empty;
            var monitor = entity.Monitor;
            if (monitor != null)
            {
                var student = _imsDataContext.Students.FirstOrDefault(r => r.Id == monitor);
                if (student != null)
                {
                    monitorName = $"{student.Name}";
                }
            }
            var dto = new ClassesDto()
            {
                Id = entity.Id,
                Dept = entity.Dept,
                Grade = entity.Grade,
                Name = entity.Name,
                HeadTeacher = entity.HeadTeacher,
                Monitor=entity.Monitor,
                MonitorName = monitorName,
                CreateTime = entity.CreateTime,
                CreateUser = entity.CreateUser,
                LastEditTime = entity.LastEditTime,
                LastEditUser = entity.LastEditUser,
                
            };
            return dto;
        }
    }
}
