using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using eCommerce.Auth;
using eCommerce.Business.Service;
using eCommerce.Common;
using NLog;

namespace eCommerce.Business
{
    public class UserManager
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private IUserAuth _auth;
        
        // token to user
        private ConcurrentDictionary<string, IUser> _connectedUsers;
        private ConcurrentDictionary<string, bool> _connectedUsersName;
        private IRepository<IUser> _registeredUsersRepo;
        private ConcurrentDictionary<string, IUser> _admins;

        private ConcurrentIdGenerator _concurrentIdGenerator;


        public UserManager(IUserAuth auth, IRepository<IUser> registeredUsersRepo)
        {
            _auth = auth;
            _connectedUsers = new ConcurrentDictionary<string, IUser>();
            _connectedUsersName = new ConcurrentDictionary<string, bool>();
            // TODO get the initialze id value from DB
            _concurrentIdGenerator = new ConcurrentIdGenerator(0);
            _registeredUsersRepo = registeredUsersRepo;
            _admins = new ConcurrentDictionary<string, IUser>();
        }

        public string Connect()
        {
            string guestUsername = GenerateGuestUsername();
            string token = _auth.GenerateToken(guestUsername);

            IUser newUser = CreateGuestUser(guestUsername);
            _connectedUsers.TryAdd(token, newUser);
            
            _logger.Info($"New guest: {guestUsername}");
            return token;
        }
        
        public void Disconnect(string token)
        {
            if (!_auth.IsValidToken(token))
            {
                _logger.Warn($"Invalid token {token}");
                if (token != null)
                {
                    _connectedUsers.TryRemove(token, out var tuser);
                }

                return;
            }

            if (_connectedUsers.TryGetValue(token, out var user)
                && user.GetState() == Guest.State)
            {
                _connectedUsers.TryRemove(token, out user);
                _logger.Info($"Guest: {user?.Username} disconnected");

            }
        }

        public async Task<Result> Register(string token, MemberInfo memberInfo, string password)
        {
            if (!_auth.IsValidToken(token))
            {
                _logger.Warn($"Invalid token {token}");
                if (token != null)
                {
                    _connectedUsers.TryRemove(token, out var tuser);
                }

                return Result.Fail("Invalid token");
            }

            if (!_connectedUsers.TryGetValue(token, out var user))
            {
                return Result.Fail("User need to be connected or logged in");
            }
            
            Result validMemberInfoRes = IsValidMemberInfo(memberInfo);
            if (validMemberInfoRes.IsFailure)
            {
                return validMemberInfoRes;
            }

            Result authRegistrationRes = await RegisterAtAuthorization(memberInfo.Username, password);
            if (authRegistrationRes.IsFailure)
            {
                return authRegistrationRes;
            }
            
            IUser newUser = new User(Member.State, memberInfo.Clone());
            if (!_registeredUsersRepo.Add(newUser))
            {
                // TODO maybe remove the user form userAuth
                _logger.Error($"User {memberInfo.Username} was able to register at Auth but already exists in " +
                    "the registered user repository");
                return Result.Fail("User already exists");
            }

            _logger.Info($"User {memberInfo.Username} was registered");
            return Result.Ok();
        }

        public void AddAdmin(MemberInfo adminInfo, string password)
        {
            IUser user = new User(Admin.State, adminInfo);
            RegisterAtAuthorization(adminInfo.Username, password).Wait();
            _registeredUsersRepo.Add(user);
            _admins.TryAdd(adminInfo.Username, user);
        }
        
        private Task<Result> RegisterAtAuthorization(string username, string password)
        {
            return _auth.Register(username, password);
        }
        
        // TODO: use the role here, how user that is admin can log in as member
        public async Task<Result<string>> Login(string guestToken, string username, string password, UserToSystemState role)
        {
            if (!_auth.IsValidToken(guestToken))
            {
                _logger.Warn($"Invalid token {guestToken}");
                if (guestToken != null)
                {
                    _connectedUsers.TryRemove(guestToken, out var tUser);
                }

                return Result.Fail<string>("Invalid token");
            }

            if (!_connectedUsers.TryGetValue(guestToken, out var guestUser) || guestUser.GetState() != Guest.State)
            {
                return Result.Fail<string>("Not connected or not guest");
            }
            
            Result authLoginRes = await _auth.Authenticate(username, password);
            if (authLoginRes.IsFailure)
            {
                return Result.Fail<string>(authLoginRes.Error);
            }

            if (_connectedUsersName.ContainsKey(username))
            {
                return Result.Fail<string>("User is already logged in");
            }
            
            string loginToken = _auth.GenerateToken(username);
            
            IUser user = _registeredUsersRepo.GetOrNull(username);
            if (user == null)
            {
                _logger.Error($"User {username} is registered in auth, but not in usermanger");
                return Result.Fail<string>("Invalid username or password");
            }

            if (!_connectedUsers.TryRemove(guestToken, out var tUser1))
            {
                return Result.Fail<string>("Guest not connected");
            }
            
            if (!_connectedUsers.TryAdd(loginToken, user) || !_connectedUsersName.TryAdd(username, true))
            {
                _logger.Error($"UserAuth created duplicate toekn(already in connected userses dictionry)");
                return Result.Fail<string>("Error");
            }
            
            _logger.Info($"User {user.Username} logged in. Token {loginToken}");
            return Result.Ok(loginToken);
        }
        
        public Result<string> Logout(string token)
        {

            if (!_auth.IsValidToken(token))
            {
                _logger.Warn($"Invalid token {token}");
                if (token != null)
                {
                    _connectedUsers.TryRemove(token, out var tUser);
                }

                return Result.Fail<string>("Invalid token");
            }
            
            if (!(_connectedUsers.TryGetValue(token, out var user) && user.GetState() != Guest.State))
            {
                return Result.Fail<string>("Guest cant logout");
            }

            if (!_connectedUsers.TryRemove(token, out var tUser1) || 
                !_connectedUsersName.TryRemove(user.Username, out var tbool))
            {
                _logger.Error($"User logout error");
            }
            else
            {
                _logger.Info($"User {tUser1.Username} logout");
            }
            
            return Result.Ok(Connect());
        }
        
        public bool IsUserConnected(string token)
        {
            if (!_auth.IsValidToken(token))
            {
                _logger.Info($"Invalid use of token {token}");
                if (token != null)
                {
                    _connectedUsers.TryRemove(token, out var tUser);
                }

                return false;
            }

            return _connectedUsers.TryGetValue(token, out var user);
        }

        public Result<IUser> GetUserIfConnectedOrLoggedIn(string token)
        {
            if (!_auth.IsValidToken(token))
            {
                _logger.Info($"Invalid use of token {token}");
                if (token != null)
                {
                    _connectedUsers.TryRemove(token, out var tUser);
                }

                return Result.Fail<IUser>("Invalid token");
            }

            if (!_connectedUsers.TryGetValue(token, out var user))
            {
                _logger.Info($"Usage of old token {token}");
                return Result.Fail<IUser>("User not connected or logged in");
            }

            return Result.Ok(user);
        }
        
        public Result<IUser> GetUserLoggedIn(string token)
        {
            if (!_auth.IsValidToken(token))
            {
                _logger.Info($"Invalid use of token {token}");
                if (token != null)
                {
                    _connectedUsers.TryRemove(token, out var tUser);
                }

                return Result.Fail<IUser>("Invalid token");
            }

            if (!_connectedUsers.TryGetValue(token, out var user))
            {
                _logger.Info($"Usage of old token {token}");
                return Result.Fail<IUser>("User not logged in");
            }

            if (user.GetState() == Guest.State)
            {
                return Result.Fail<IUser>("This is a guest user");
            }
            
            return Result.Ok(user);
        }
        
        /// <summary>
        /// GetDiscount the user
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>The user</returns>
        public Result<IUser> GetUser(string username)
        {
            IUser user = _registeredUsersRepo.GetOrNull(username);
            if (user == null)
            {
                return Result.Fail<IUser>("User doesn't exists");
            }

            return Result.Ok(user);
        }

        private IUser CreateGuestUser(string guestName)
        {
            return new User(guestName);
        }
        
        /// <summary>
        /// Check if the member information is valid
        /// </summary>
        /// <returns>Result of the check</returns>
        private Result IsValidMemberInfo(MemberInfo memberInfo)
        {
            Result fullDataRes = memberInfo.IsBasicDataFull();
            if (fullDataRes.IsFailure)
            {
                return fullDataRes;
            }

            if (!IsValidUsername(memberInfo.Username))
            {
                return Result.Fail("Invalid username address");
            }

            if (!RegexUtils.IsValidEmail(memberInfo.Email))
            {
                return Result.Fail("Invalid email address");
            }

            return Result.Ok();
        }

        private bool IsValidUsername(string username)
        {
            return Regex.IsMatch(username,
            "^[a-zA-z][a-zA-z0-9]*$");
        }

        private long GetAndIncrementGuestId()
        {
            return _concurrentIdGenerator.MoveNext();
        }
        
        private string GenerateGuestUsername()
        {
            return $"_Guest{GetAndIncrementGuestId():D}";
        }

        public void CreateMainAdmin()
        {
            AppConfig config = AppConfig.GetInstance();
            
            MemberInfo adminInfo = new MemberInfo(
                config.GetData("AdminCreationInfo:Username"),
                config.GetData("AdminCreationInfo:Email"),
                "TheAdmin",
                DateTime.Now, 
                null);
            AddAdmin(adminInfo, config.GetData("AdminCreationInfo:Password"));
        }
    }
}