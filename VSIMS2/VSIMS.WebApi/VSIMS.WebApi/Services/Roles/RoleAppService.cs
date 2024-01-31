using VSIMS.WebApi.Entity;
using VSIMS.WebApi.Data;
using VSIMS.WebApi.Utils;

namespace VSIMS.WebApi.Services.Roles
{
    public class RoleAppService : IRoleAppService
    {
        private ImsDataContext _imsDataContext;

        public RoleAppService(ImsDataContext imsDataContext)
        {
            this._imsDataContext = imsDataContext;
        }

        public int AddRole(RoleEntity role)
        {
            var entry = _imsDataContext.Roles.Add(role);
            _imsDataContext.SaveChanges();
            return 0;
        }

        public int DeleteRole(int id)
        {
            var entity = _imsDataContext.Roles.FirstOrDefault(x => x.Id == id);
            if (entity != null)
            {
                _imsDataContext.Roles.Remove(entity);
                _imsDataContext.SaveChanges();
            }
            return 0;
        }

        public RoleEntity GetRole(int id)
        {
            var entity = _imsDataContext.Roles.FirstOrDefault(r => r.Id == id);
            return entity;
        }

        public PagedRequest<RoleEntity> GetRoles(string roleName, int pageNum, int pageSize)
        {
            IQueryable<RoleEntity> roles = null;
            if (!string.IsNullOrEmpty(roleName))
            {
                roles = _imsDataContext.Roles.Where(r => r.Name.Contains(roleName)).OrderBy(r => r.Id);
            }
            else
            {
                roles = _imsDataContext.Roles.Where(r => true).OrderBy(r => r.Id);
            }
            int count = roles.Count();
            List<RoleEntity> items;
            if (pageSize > 0)
            {
                items = roles.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                items = roles.ToList();
            }
            return new PagedRequest<RoleEntity>()
            {
                count = count,
                items = items
            };
        }

        public int UpdateRole(RoleEntity role)
        {
            _imsDataContext.Roles.Update(role);
            _imsDataContext.SaveChanges();
            return 0;
        }

        public PagedRequest<UserRight> GetUserRights(int? userId)
        {
            if (userId != null)
            {
                var query = from u in _imsDataContext.UserRoles
                            join r in _imsDataContext.Roles on u.RoleId equals r.Id
                            join x in _imsDataContext.RoleMenus on r.Id equals x.RoleId
                            join m in _imsDataContext.Menus on x.MenuId equals m.Id
                            where u.UserId == userId
                            select new UserRight { Id = m.Id, RoleName = r.Name, MenuName = m.Name, Url = m.Url,Icon=m.Icon, ParentId = m.ParentId, SortId = m.SortId };
                return new PagedRequest<UserRight>()
                {
                    count = query.Count(),
                    items = query.ToList()
                };
            }
            return null;
        }

        public PagedRequest<RoleMenuEntity> GetRoleMenus(int? roleId)
        {
            if (roleId != null)
            {
                var roleMenus = this._imsDataContext.RoleMenus.Where(x => x.RoleId == roleId);
                return new PagedRequest<RoleMenuEntity>()
                {
                    count = roleMenus.Count(),
                    items = roleMenus.ToList()
                };
            }
            else
            {
                return null;
            }
        }

        public int SetRoleMenus(int roleId, string menuIds)
        {
            string[] menus = menuIds.Split(',');
            if (menus.Length == 0)
            {
                return -1;//权限为空
            }
            if (roleId < 1)
            {
                return -1;
            }
            var oldRoleMenus = _imsDataContext.RoleMenus.Where(r => r.RoleId == roleId);
            if (oldRoleMenus.Count() > 0)
            {
                this._imsDataContext.RoleMenus.RemoveRange(oldRoleMenus);
            }
            var entities = menus.Select(r => new RoleMenuEntity()
            {
                RoleId = roleId,
                MenuId =int.Parse(r),
                CreateTime = DateTime.Now,
                LastEditTime = DateTime.Now,
            });
            this._imsDataContext.RoleMenus.AddRange(entities);
            this._imsDataContext.SaveChanges();
            return 0;
        }
    }
}
