using System;
using System.IO;
using eCommerce.Business;
using eCommerce.Common;
using eCommerce.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace eCommerce
{
    public class InitDataJsonFormat {
        public string Type { get; set; }
        public JObject Data { get; set; }
    }
    
    public class CreateUserData {
        public MemberInfo MemberInfo { get; set; }
        public string Password { get;  set; }
    }
    
    public class InitSystem
    {
        private IAuthService _authService;
        private IUserService _userService;
        private IStoreService _storeService;

        private string _guestToken;
        public InitSystem()
        {
            _authService = new AuthService();
            _userService = new UserService();
            _storeService = new StoreService();

            _guestToken = _authService.Connect();
        }

        private void Setup()
        {
            _guestToken = _authService.Connect();
        }
        
        private void CleanUp()
        { 
            _authService.Disconnect(_guestToken);
        }

        public void Init(string initFile)
        {
            Setup();
            
            int at = 0;
            InitDataJsonFormat[] initDataJsons = JsonConvert.DeserializeObject<InitDataJsonFormat[]>(File.ReadAllText(initFile));

            foreach (var initData in initDataJsons)
            {
                switch (initData.Type)
                {
                    case "CreateUser":
                    {
                        CreateUserData userData = initData.Data.ToObject<CreateUserData>();
                        Result registerRes = _authService.Register(_guestToken, userData.MemberInfo, userData.Password);
                        if (registerRes.IsFailure)
                        {
                            throw new InvalidDataException(
                                $"\nSystem Init:\nError when register user {userData.MemberInfo.Username}\n" +
                                $"Error message: {registerRes.Error}\n" +
                                $"In index {at}, file {initFile}");
                        }
                        break;
                    }
                    case "AddStore":
                    {
                        Console.WriteLine($"AddStore {initData.Data}");
                        break;
                    }
                    case "AddItemToStore":
                    {
                        Console.WriteLine($"AddItemToStore {initData.Data}");
                        break;
                    }
                        
                }

                at++;
            }

            CleanUp();
        }
    }
}