using VSIMS.WebApi.Entity;
using VSIMS.WebApi.Data;
using VSIMS.WebApi.Utils;

namespace VSIMS.WebApi.Services.Users
{
    public class UserAppService : IUserAppService
    {
        private ImsDataContext _imsDataContext;

        public UserAppService(ImsDataContext imsDataContext)
        {
            this._imsDataContext = imsDataContext;
        }

        public int AddUser(UserEntity user)
        {
            var entry = _imsDataContext.Users.Add(user);
            _imsDataContext.SaveChanges();
            return 0;
        }

        public int DeleteUser(int id)
        {
            var entity = _imsDataContext.Users.FirstOrDefault(x => x.Id == id);
            if (entity != null)
            {
                _imsDataContext.Users.Remove(entity);
                _imsDataContext.SaveChanges();
            }
            return 0;
        }

        public UserEntity GetUser(int id)
        {
            var entity = _imsDataContext.Users.FirstOrDefault(r => r.Id == id);
            return entity;
        }

        /// <summary>
        /// 获取用户角色列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public PagedRequest<UserRoleEntity> GetUserRoles(int? userId)
        {
            if (userId != null)
            {
                var userRoles = this._imsDataContext.UserRoles.Where(x => x.UserId == userId);
                return new PagedRequest<UserRoleEntity>()
                {
                    count = userRoles.Count(),
                    items = userRoles.ToList()
                };
            }
            else {
                return null;
            }
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PagedRequest<UserEntity> GetUsers(string userName, int pageNum, int pageSize)
        {
            IQueryable<UserEntity> users = null;
            if (!string.IsNullOrEmpty(userName))
            {
                users = _imsDataContext.Users.Where(r => r.UserName.Contains(userName)).OrderBy(r => r.Id);
            }
            else
            {
                users = _imsDataContext.Users.Where(r => true).OrderBy(r => r.Id);
            }
            int count = users.Count();
            List<UserEntity> items;
            if (pageSize > 0)
            {
                items = users.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                items = users.ToList();
            }
            return new PagedRequest<UserEntity>()
            {
                count = count,
                items = items
            };
        }

        public int SetUserRoles(int userId, string roleIds)
        {
            string[] roles = roleIds.Split(',');
            if (roles.Length == 0) { 
                return -1;//权限为空
            }
            if (userId <1) {
                return -1;
            }
            var oldUserRoles = _imsDataContext.UserRoles.Where(r => r.UserId == userId);
            if (oldUserRoles.Count() > 0) { 
                this._imsDataContext.UserRoles.RemoveRange(oldUserRoles);
            }
            var entities = roles.Select(r => new UserRoleEntity()
            {
                UserId = userId,
                RoleId = int.Parse(r),
                CreateTime = DateTime.Now,
                LastEditTime = DateTime.Now,
            });
            this._imsDataContext.UserRoles.AddRange(entities);
            this._imsDataContext.SaveChanges();
            return 0;
        }

        public int UpdateUser(UserEntity user)
        {
            _imsDataContext.Users.Update(user);
            _imsDataContext.SaveChanges();
            return 0;
        }
    }
}
