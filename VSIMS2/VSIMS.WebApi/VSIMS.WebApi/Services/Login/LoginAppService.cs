using VSIMS.WebApi.Data;

namespace VSIMS.WebApi.Services.Login
{
    public class LoginAppService : ILoginAppService
    {
        private ImsDataContext _imsDataContext;

        public LoginAppService(ImsDataContext imsDataContext)
        {
            this._imsDataContext = imsDataContext;
        }

        public int? Login(string username, string password)
        {
            var item = _imsDataContext.Users.FirstOrDefault(i => i.UserName == username && i.Password == password);
            return item?.Id;
        }
    }
}
