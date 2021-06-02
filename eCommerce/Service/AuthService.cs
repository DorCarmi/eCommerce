using System;
using System.Threading.Tasks;
using eCommerce.Auth;
using eCommerce.Business;
using eCommerce.Common;

namespace eCommerce.Service
{
    public class AuthService : IAuthService
    {
        private IMarketFacade _marketFacade;
        
        internal AuthService(IMarketFacade marketFacade)
        {
            _marketFacade = MarketFacade.GetInstance();
        }
        
        public AuthService()
        {
            _marketFacade = MarketFacade.GetInstance();
        }

        public static AuthService CreateUserServiceForTests(IUserAuth userAuth,
            IRepository<User> registeredUsersRepo,
            StoreRepository storeRepo)
        {
            IMarketFacade marketFacade = MarketFacade.CreateInstanceForTests(userAuth, registeredUsersRepo, storeRepo);
            return new AuthService(marketFacade);
        }
        
        public string Connect()
        {
            return _marketFacade.Connect();
        }

        public void Disconnect(string token)
        {
            // var arr=Enum.GetValues(typeof(StorePermission));
            // foreach (var a in arr)
            // {
            //     a.ToString();
            // }
            _marketFacade.Disconnect(token);
        }

        //TODO: maybe user dto for MemberInfo
        public Task<Result> Register(string token, MemberInfo memberInfo, string password)
        {
            return _marketFacade.Register(token, memberInfo, password);
        }

        public Task<Result<string>> Login(string guestToken, string username, string password, ServiceUserRole role)
        {
            return _marketFacade.Login(guestToken, username, password,
                DtoUtils.ServiceUserRoleToSystemState(role));
        }

        public Result<string> Logout(string token)
        {
            return _marketFacade.Logout(token);
        }

        public bool IsUserConnected(string token)
        {
            return _marketFacade.IsUserConnected(token);
        }
    }
}