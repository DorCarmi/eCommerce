using System;
using System.IO;
using eCommerce.Adapters;
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

            InitPaymentAdapter(config);
            InitSupplyAdapter(config);
            
            marketFacade = MarketFacade.GetInstance();
            marketFacade.Init(authService, userRepo, storeRepo);
        }

        private IUserAuth InitAuth(AppConfig config)
        {
            IUserAuth authService = UserAuth.GetInstance();
            authService.Init(config);
            return authService;
        }

        private void InitPaymentAdapter(AppConfig config)
        {
            string paymentAdapter = config.GetData("PaymentAdapter");
            switch (paymentAdapter)
            {
                case "WSEP":
                {
                    PaymentProxy.AssignPaymentService(new WSEPPaymentAdapter());
                    break;
                }
                case null:
                {
                    break;
                }
                default:
                {
                    config.ThrowErrorOfData("PaymentAdapter", "invalid");
                    break;
                }
                    
            }
        }
        
        private void InitSupplyAdapter(AppConfig config)
        {
            string paymentAdapter = config.GetData("SupplyAdapter");
            switch (paymentAdapter)
            {
                case "WSEP":
                {
                    SupplyProxy.AssignSupplyService(new WSEPSupplyAdapter());
                    break;
                }
                case null:
                {
                    break;
                }
                default:
                {
                    config.ThrowErrorOfData("SupplyAdapter", "invalid");
                    break;
                }
                    
            }
        }
    }
}