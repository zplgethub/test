

using VSIMS.WebApi.Data;
using VSIMS.WebApi.Entity;
using VSIMS.WebApi.Utils;

namespace VSIMS.WebApi.Services.Menus
{
    public class MenuAppService : IMenuAppService
    {
        private ImsDataContext _imsDataContext;

        public MenuAppService(ImsDataContext imsDataContext)
        {
            this._imsDataContext = imsDataContext;
        }

        public int AddMenu(MenuEntity menu)
        {
            var entry = _imsDataContext.Menus.Add(menu);
            _imsDataContext.SaveChanges();
            return 0;
        }

        public int DeleteMenu(int id)
        {
            var entity = _imsDataContext.Menus.FirstOrDefault(x => x.Id == id);
            if (entity != null)
            {
                _imsDataContext.Menus.Remove(entity);
                _imsDataContext.SaveChanges();
            }
            return 0;
        }

        public MenuEntity GetMenu(int id)
        {
            var entity = _imsDataContext.Menus.FirstOrDefault(r => r.Id == id);
            return entity;
        }

        public PagedRequest<MenuEntity> GetMenus(string menuName, int pageNum, int pageSize)
        {
            IQueryable<MenuEntity> menus = null;
            if (!string.IsNullOrEmpty(menuName))
            {
                menus = _imsDataContext.Menus.Where(r => r.Name.Contains(menuName)).OrderBy(r => r.Id);
            }
            else
            {
                menus = _imsDataContext.Menus.Where(r => true).OrderBy(r => r.Id);
            }
            int count = menus.Count();
            List<MenuEntity> items;
            if (pageSize > 0)
            {
                items = menus.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                items = menus.ToList();
            }
            return new PagedRequest<MenuEntity>()
            {
                count = count,
                items = items
            };
        }

        public int UpdateMenu(MenuEntity menu)
        {
            _imsDataContext.Menus.Update(menu);
            _imsDataContext.SaveChanges();
            return 0;
        }
    }
}
