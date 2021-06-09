using System;
using System.IO;
using eCommerce.Auth;
using eCommerce.Business;
using eCommerce.Business.Repositories;
using eCommerce.Common;

namespace eCommerce.Service
{
    public class SystemService: ISystemService
    {

        private MarketState _marketState;
        
        public SystemService()
        {
            _marketState = MarketState.GetInstance();
        }
        
        public bool IsSystemValid()
        {
            return _marketState.ValidState;
        }

        public bool GetErrMessageIfValidSystem(out string message)
        {
            return _marketState.TryGetErrMessage(out message);
        }

        public void InitSystem()
        {
            AppConfig config = AppConfig.GetInstance();

            MarketFacade marketFacade;
            IUserAuth authService = InitAuth(config);
            IRepository<User> userRepo = null;
            AbstractStoreRepo storeRepo = null;

            string memoryAs = config.GetData("Memory");
            switch (memoryAs)
            {
                case "InMemory":
                {
                    userRepo = new InMemoryRegisteredUsersRepository();
                    storeRepo = new InMemoryStoreRepo();
                    break;
                }
                case "Persistence":
                {
                    throw new NotImplementedException();
                    break;   
                }
                case null:
                {
                    config.ThrowErrorOfData("Memory", "missing");
                    break;
                }
                default:
                {
                    config.ThrowErrorOfData("Memory", "invalid");
                    break;
                }
            }
            
            marketFacade = MarketFacade.GetInstance();
            marketFacade.Init(authService, userRepo, storeRepo);
        }

        private IUserAuth InitAuth(AppConfig config)
        {
            IUserAuth authService = UserAuth.GetInstance();
            authService.Init(config);
            return authService;
        }
    }
}