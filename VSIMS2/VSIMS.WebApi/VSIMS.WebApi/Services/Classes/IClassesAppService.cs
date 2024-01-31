using VSIMS.WebApi.Dto;
using VSIMS.WebApi.Entity;
using VSIMS.WebApi.Utils;

namespace VSIMS.WebApi.Services.Classes
{
    public interface IClassesAppService
    {
        public PagedRequest<ClassesDto> GetClassess(string dept, string grade, int pageNum, int pageSize);

        /// <summary>
        /// 通过id查询班级信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ClassesEntity GetClasses(int id);

        /// <summary>
        /// 新增班级
        /// </summary>
        /// <param name="classes"></param>
        /// <returns></returns>
        public int AddClasses(ClassesEntity classes);

        /// <summary>
        /// 修改班级
        /// </summary>
        /// <param name="classes"></param>
        /// <returns></returns>
        public int UpdateClasses(ClassesEntity classes);

        /// <summary>
        /// 删除班级
        /// </summary>
        /// <param name="id"></param>
        public int DeleteClasses(int id);
    }
}
