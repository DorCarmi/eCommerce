using System;
using System.IO;
using eCommerce.Business;
using eCommerce.Common;
using eCommerce.Service;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace eCommerce
{
    public class BasicActionJsonFormat {
        public string Action { get; set; }
        public JObject Data { get; set; }
    }
    
    public class CreateUserData {
        public MemberInfo MemberInfo { get; set; }
        public string Password { get;  set; }
    }
    
    public class MemberAction {
        public string Username { get; set; }
        public string Password { get; set; }
        public BasicActionJsonFormat[] Actions { get; set; }
    }

    public class InitSystem
    {
        private ISystemService _systemService;
        private IAuthService _authService;
        private IUserService _userService;
        private INStoreService _inStoreService;

        private AppConfig _appConfig;

        private string _guestToken;
        private string _workingToken;
        public InitSystem()
        {
            _systemService = new SystemService();
            _authService = new AuthService();
            _userService = new UserService();
            _inStoreService = new InStoreService();
            _appConfig = AppConfig.GetInstance();

            _guestToken = _authService.Connect();
        }

        private void Setup()
        {
            _guestToken = _authService.Connect();
        }
        
        private void CleanUp()
        { 
            _authService.Disconnect(_guestToken);
            _authService.Disconnect(_authService.Logout(_workingToken).Value);
        }

        public void Init(string initFile)
        {
            if (_appConfig.GetData("InitWithData").Equals("True"))
            {
                InitData("Init.json");
            }
        }

        private void InitData(string initFile)
        {
            Setup();
            
            int at = 0;
            BasicActionJsonFormat[] initDataJsons = JsonConvert.DeserializeObject<BasicActionJsonFormat[]>(File.ReadAllText(initFile));

            foreach (var initData in initDataJsons)
            {
                switch (initData.Action)
                {
                    case "CreateUser":
                    {
                        CreateUserData userData = initData.Data.ToObject<CreateUserData>();
                        Result registerRes = _authService.Register(_guestToken, userData.MemberInfo, userData.Password).Result;
                        if (registerRes.IsFailure)
                        {
                            ThrowInvalidException($"Error when register user {userData.MemberInfo.Username}",
                                registerRes.Error, at, initFile);
                        }
                        break;
                    }
                    case "MemberAction":
                    {
                        MemberAction memberAction = initData.Data.ToObject<MemberAction>();
                        HandleMemberActions(memberAction, at, initFile);
                        break;
                    }
                    default:
                        throw new InvalidDataException($"Invalid init action {initData.Action}");
                        
                }

                at++;
            }

            CleanUp();
        }

        private void HandleMemberActions(MemberAction memberAction, int at, string initFile)
        {
            _workingToken = _authService.Connect();
            Result<string> loginRes = _authService.Login(_workingToken, memberAction.Username,
                memberAction.Password, ServiceUserRole.Member).Result;
            if (loginRes.IsFailure)
            {
                ThrowInvalidException($"User {memberAction.Username} wasn't able to login",
                    loginRes.Error, at, initFile);
            }

            _workingToken = loginRes.Value;
            
            foreach (var basicAction in memberAction.Actions)
            {
                switch (basicAction.Action)
                {
                    case "OpenStore":
                    {
                        string storeName = basicAction.Data["StoreName"].ToString();
                        Result openStoreRes = _inStoreService.OpenStore(_workingToken, storeName);
                        if (openStoreRes.IsFailure)
                        {
                            ThrowInvalidException($"Error when opening store {storeName} for {memberAction.Username}",
                                openStoreRes.Error, at, initFile);
                        }
                        break;
                    }
                    case "AddItem":
                    {
                        SItem item = basicAction.Data.ToObject<SItem>();
                        Result addItemRes = _inStoreService.AddNewItemToStore(_workingToken, item);
                        if (addItemRes.IsFailure)
                        {
                            ThrowInvalidException($"Error when adding item {item.ItemName} to store {item.StoreName}",
                                addItemRes.Error, at, initFile);
                        }
                        break;
                    }
                    default:
                        throw new InvalidDataException($"Invalid init action at index {at} of MemberAction");
                }
            }
            
            _authService.Disconnect(_authService.Logout(_workingToken).Value);
        }

        private void ThrowInvalidException(string errorMessage, string resMessage, int at, string initFile)
        {
            CleanUp();
            throw new InvalidDataException(
                $"\nSystem Init:\n{errorMessage}\n" +
                $"Error message: {resMessage}\n" +
                $"In index {at}, file {initFile}");
        }
    }
}