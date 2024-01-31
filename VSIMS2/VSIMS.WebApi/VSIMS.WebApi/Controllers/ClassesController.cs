using Microsoft.AspNetCore.Mvc;
using VSIMS.WebApi.Dto;
using VSIMS.WebApi.Entity;
using VSIMS.WebApi.Services.Classes;
using VSIMS.WebApi.Utils;

namespace VSIMS.WebApi.Controllers
{
    /// <summary>
    /// 班级控制器
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly ILogger<ClassesController> logger;

        private readonly IClassesAppService classesAppService;

        public ClassesController(ILogger<ClassesController> logger, IClassesAppService classesAppService)
        {
            this.logger = logger;
            this.classesAppService = classesAppService;
        }

        /// <summary>
        /// 获取班级信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ClassesEntity GetClasses(int id)
        {
            return classesAppService.GetClasses(id);
        }

        /// <summary>
        /// 获取班级列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public PagedRequest<ClassesDto> GetClassess(string? dept, string? grade, int pageNum, int pageSize)
        {
            return classesAppService.GetClassess(dept,grade,pageNum,pageSize);
        }

        /// <summary>
        /// 新增班级
        /// </summary>
        /// <param name="classes"></param>
        /// <returns></returns>
        [HttpPost]
        public int AddClasses(ClassesEntity classes)
        {
            return classesAppService.AddClasses(classes);
        }

        /// <summary>
        /// 修改班级
        /// </summary>
        /// <param name="classes"></param>
        /// <returns></returns>
        [HttpPost]
        public int UpdateClasses(ClassesEntity classes)
        {
            return classesAppService.UpdateClasses(classes);
        }

        /// <summary>
        /// 删除班级
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete]
        public int DeleteClasses(int id)
        {
            return classesAppService.DeleteClasses(id);
        }
    }
}
