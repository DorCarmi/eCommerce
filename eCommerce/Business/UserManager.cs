using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using eCommerce.Auth;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class UserManager
    {
        private IUserAuth _auth;
        // Token to user
        private ConcurrentDictionary<string, IUser> _connectedUsers;
        private IRepository<IUser> _registeredUsersRepo;


        public UserManager(IUserAuth auth, IRepository<IUser> registeredUsersRepo)
        {
            _auth = auth;
            _connectedUsers = new ConcurrentDictionary<string, IUser>();
            _registeredUsersRepo = registeredUsersRepo;
        }

        public string Connect()
        {
            string token = _auth.Connect();
            Result<AuthData> userAuthDataRes = _auth.GetData(token);
            if (userAuthDataRes.IsFailure)
            {
                // log it Authorization connect returned not valid token
                return null;
            }

            string username = userAuthDataRes.Value.Username;
            IUser newUser = CreateGuestUser(username);
            _connectedUsers.TryAdd(token, newUser);
            return token;
        }
        
        public void Disconnect(string token)
        {
            if (!_auth.IsValidToken(token))
            {
                return;
            }
            
            _auth.Disconnect(token);
            _connectedUsers.Remove(token, out var user);
            if (user == null)
            {
                // user using old token
            }
        }

        public Result Register(string token, MemberInfo memberInfo, string password)
        {
            
            if (!_connectedUsers.ContainsKey(token))
            {
                return Result.Fail("Need to be connected or logged in");
            }
            
            Result validMemberInfoRes = IsValidMemberInfo(memberInfo);
            if (validMemberInfoRes.IsFailure)
            {
                return validMemberInfoRes;
            }

            Result authRegistrationRes = RegisterAtAuthorization(memberInfo.Username, password);
            if (authRegistrationRes.IsFailure)
            {
                return authRegistrationRes;
            }
            
            IUser newUser = new User(Member.State, memberInfo.Clone());
            if (!_registeredUsersRepo.Add(newUser))
            {
                // TODO maybe remove the user form userAuth and log it
                return Result.Fail("User already exists");
            }

            return Result.Ok();
        }

        public void AddAdmin(MemberInfo adminInfo, string password)
        {
            IUser user = new User(Admin.State, adminInfo);
            RegisterAtAuthorization(adminInfo.Username, password);
            _registeredUsersRepo.Add(user);
        }
        
        private Result RegisterAtAuthorization(string username, string password)
        {
            return _auth.Register(username, password);
        }
        
        public Result<string> Login(string guestToken, string username, string password, ServiceUserRole role)
        {
            Result<string> guestRes = IsConnectedGuest(guestToken);
            if (guestRes.IsFailure)
            {
                return Result.Fail<string>(guestRes.Error);;
            }
            
            Result<string> authLoginRes = _auth.Login(username, password, DtoUtils.ServiceUserRoleToAuthUserRole(role));
            if (authLoginRes.IsFailure)
            {
                return Result.Fail<string>(authLoginRes.Error);
            }
            
            IUser user = _registeredUsersRepo.GetOrNull(username);
            if (user == null)
            {
                // TODO log it 
                _auth.Logout(authLoginRes.Value);
                return Result.Fail<string>("Invalid username or password");
            }

            _connectedUsers.Remove(guestToken, out var guestUser);
            if (!_connectedUsers.TryAdd(authLoginRes.Value, user))
            {
                _auth.Logout(authLoginRes.Value);
                return Result.Fail<string>("User already logged in");
            }
            
            _auth.Disconnect(guestToken);
            return Result.Ok(authLoginRes.Value);
        }
        
        public Result<string> Logout(string token)
        {
            Result<string> usernameRes = IsLoggedIn(token);
            if (usernameRes.IsFailure)
            {
                // TODO log it
                return usernameRes;
            }
            
            if (!_connectedUsers.TryGetValue(token, out IUser user))
            {
                // TODO log it
                return Result.Fail<string>("Trying to use old token");
            }

            _auth.Logout(token);

            string newGuestToken = _auth.Connect();
            string newGuestUsername = _auth.GetData(newGuestToken).Value.Username;
            
            _connectedUsers.Remove(token, out var tuser);
            _connectedUsers.TryAdd(newGuestToken, new User(newGuestUsername));
            
            return Result.Ok(newGuestToken);
        }

        public Result<IUser> GetUserIfConnectedOrLoggedIn(string token)
        {
            if (!_auth.IsValidToken(token))
            {
                return Result.Fail<IUser>("Invalid token");
            }

            if (!_connectedUsers.TryGetValue(token, out var user))
            {
                return Result.Fail<IUser>("User not connected or loggedin");
            }

            return Result.Ok(user);
        }
        
        /// <summary>
        /// Get the user
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>The user</returns>
        public Result<IUser> GetUser(string username)
        {
            IUser user = _registeredUsersRepo.GetOrNull(username);
            if (user == null)
            {
                // TODO log it 
                return Result.Fail<IUser>("User doesn't exists");
            }

            return Result.Ok(user);
        }
        
        private IUser GetConnectedUserOrNull(string username)
        {
            _connectedUsers.TryGetValue(username, out var user);
            if (user == null)
            {
                // TODO log it
                return null;
            }

            return user;
        }

        private IUser CreateGuestUser(string guestName)
        {
            // TODO update it with user implementation
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

        private Result<string> IsConnectedGuest(string token)
        {
            Result<AuthData> authDataRes = _auth.GetData(token);
            if (authDataRes.IsFailure)
            {
                return Result.Fail<string>(authDataRes.Error);
            }

            if (!authDataRes.Value.Role.Equals(_auth.RoleToString(AuthUserRole.Guest)))
            {
                return Result.Fail<string>("Not a guest");
            }

            return Result.Ok(authDataRes.Value.Username);
        }
        
        /// <summary>
        /// Check if the token is valid and the role is
        /// not a guest therefore the user can be considered as logged in
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private Result<string> IsLoggedIn(string token)
        {
            Result<AuthData> authDataRes = _auth.GetData(token);
            if (authDataRes.IsFailure)
            {
                return Result.Fail<string>(authDataRes.Error);
            }

            if (authDataRes.Value.Role.Equals(_auth.RoleToString(AuthUserRole.Guest)))
            {
                return Result.Fail<string>("A guest");
            }

            return Result.Ok(authDataRes.Value.Username);
        }
    }
}