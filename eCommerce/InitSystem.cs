using System;
using System.Collections.Generic;
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
    
    public class AppointManager {
        public string Manager { get; set; }
        public string Store { get; set; }
        public string[] Permissions { get; set; }
    }

    public class InitSystem
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
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
            _appConfig = AppConfig.GetInstance();

        }

        private void InitServices()
        {
            _authService = new AuthService();
            _userService = new UserService();
            _inStoreService = new InStoreService();

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
            
            _systemService.InitSystem();
            InitServices();
            
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
                            LogError($"Error when register user {userData.MemberInfo.Username}",
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
                        ThrowInvalidException($"Invalid init action {initData.Action}",
                            "Invalid action", at, initFile);
                        break;
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
                LogError($"User {memberAction.Username} wasn't able to login",
                    loginRes.Error, at, initFile);
                return;
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
                            LogError($"Error when opening store {storeName} for {memberAction.Username}",
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
                            LogError($"Error when adding item {item.ItemName} to store {item.StoreName}",
                                addItemRes.Error, at, initFile);
                        }
                        break;
                    }
                    case "AppointManager":
                    {
                        AppointManager appointManager = basicAction.Data.ToObject<AppointManager>();
                        Result appointManagerRes = _userService.AppointManager(_workingToken, appointManager.Store, appointManager.Manager);
                        if (appointManagerRes.IsFailure)
                        {
                            LogError($"Error when appointing {appointManager.Manager} to store {appointManager.Store}",
                                appointManagerRes.Error, at, initFile);
                            break;
                        }

                        if (appointManager.Permissions != null && appointManager.Permissions.Length > 0)
                        {
                            List<StorePermission> managerPermissions = new List<StorePermission>();
                            foreach (var permission in appointManager.Permissions)
                            {
                                try
                                {
                                    StorePermission storePermission = Enum.Parse<StorePermission>(permission, true);
                                    managerPermissions.Add(storePermission);
                                }
                                catch (Exception e)
                                {
                                    LogError($"Invalid permission {permission} when appointing {appointManager.Permissions} to store {appointManager.Store}",
                                        appointManagerRes.Error, at, initFile);
                                }
                            }

                            Result updateManagerPermission = _userService.UpdateManagerPermission(_workingToken, appointManager.Store,
                                appointManager.Manager, managerPermissions);
                            if (updateManagerPermission.IsFailure)
                            {
                                LogError($"Error when updating {appointManager.Manager} manager permission on store {appointManager.Store}",
                                    appointManagerRes.Error, at, initFile);
                            }
                        }
                        break;
                    }
                    default:
                        ThrowInvalidException($"Invalid init action at index {at} of MemberAction",
                            "Invalid action", at, initFile);
                        break;
                }
            }
            
            _authService.Disconnect(_authService.Logout(_workingToken).Value);
        }

        private void LogError(string errorMessage, string resMessage, int at, string initFile)
        {
            CleanUp();
            string message = $"\nSystem Init:\n{errorMessage}\n" +
                             $"Error message: {resMessage}\n" +
                             $"In index {at}, file {initFile}";
            _logger.Error(message);
        }
        
        private void ThrowInvalidException(string errorMessage, string resMessage, int at, string initFile)
        {
            CleanUp();
            string message = $"\nSystem Init:\n{errorMessage}\n" +
                             $"Error message: {resMessage}\n" +
                             $"In index {at}, file {initFile}";
            _logger.Error(message);
            throw new InvalidDataException(message);
        }
    }
}