using eCommerce.Business;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Service
{
    public class AuthService : IAuthService
    {
        private IMarketFacade _marketFacade;
        
        public AuthService()
        {
            _marketFacade = MarketFacade.GetInstance();
        }
        
        public string Connect()
        {
            return _marketFacade.Connect();
        }

        public void Disconnect(string token)
        {
            _marketFacade.Disconnect(token);
        }

        //TODO: maybe user dto for MemberInfo
        public Result Register(string token, MemberInfo memberInfo, string password)
        {
            return _marketFacade.Register(token, memberInfo, password);
        }

        public Result<string> Login(string guestToken, string username, string password, ServiceUserRole role)
        {
            return _marketFacade.Login(guestToken, username, password,
                DtoUtils.ServiceUserRoleToSystemState(role));
        }

        public Result<string> Logout(string token)
        {
            return _marketFacade.Logout(token);
        }
    }
}