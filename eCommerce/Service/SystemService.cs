using System;
using eCommerce.Adapters;
using eCommerce.Auth;
using eCommerce.Business;
using eCommerce.Business.Repositories;
using eCommerce.Common;
using eCommerce.Statistics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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

        public void Start(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        public bool InitSystem(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: <Config file>");
                return false;
            }
            
            AppConfig config = AppConfig.GetInstance();
            if (!config.Init(args[0]))
            {
                Console.WriteLine($"Usage: Invalid config file {args[0]}");
                return false;
            }

            MarketFacade marketFacade;
            IUserAuth authService;
            IRepository<User> userRepo = null;
            AbstractStoreRepo storeRepo = null;

            authService = InitAuth(config);
            InitStatistics(config);
            InitPaymentAdapter(config);
            InitSupplyAdapter(config);
            
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
                    userRepo = new PersistenceRegisteredUsersRepo();
                    //TODO update to persistennce store repo
                    storeRepo = new InMemoryStoreRepo();
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

            string initFilePath;
            if (config.GetData("InitWithData").Equals("True"))
            {
                initFilePath = config.GetData("InitDataFile");
                if (initFilePath != null)
                {
                    InitSystemWithData initSystemWithData = new InitSystemWithData(
                        new AuthService(),
                        new UserService(),
                        new InStoreService());
                    initSystemWithData.Init(initFilePath);
                }
            }

            return true;
        }

        private IUserAuth InitAuth(AppConfig config)
        {
            IUserAuth authService = UserAuth.GetInstance();
            authService.Init(config);
            return authService;
        }
        
        private IStatisticsService InitStatistics(AppConfig config)
        {
            IStatisticsService statisticsService = Statistics.Statistics.GetInstance();
            statisticsService.Init(config);
            return statisticsService;
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