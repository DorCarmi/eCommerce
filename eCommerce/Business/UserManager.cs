using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using eCommerce.Auth;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class UserManager
    {
        private IUserAuth _auth;
        private ConcurrentDictionary<string, IUser> _connectedUsers;
        private IRepository<IUser> _registeredUsersRepo;


        public UserManager(IUserAuth auth, IRepository<IUser> registeredUsersRepo)
        {
            _auth = auth;
            _connectedUsers = new ConcurrentDictionary<string, IUser>();
            _registeredUsersRepo = registeredUsersRepo;
        }

        public string CreateNewGuestConnection()
        {
            string token = _auth.Connect();
            Result<AuthData> userAuthDataRes = _auth.GetDataIfConnected(token);
            if (userAuthDataRes.IsFailure)
            {
                // log it Authorization connect returned not valid token
                return null;
            }

            string username = userAuthDataRes.Value.Username;
            IUser newUser = CreateGuestUser(username);
            _connectedUsers.TryAdd(username, newUser);
            return token;
        }
        
        public void Disconnect(string token)
        {
            Result<AuthData> authData = _auth.GetDataIfConnected(token);
            if (authData.IsFailure)
            {
                return;
            }
            
            _auth.Disconnect(token);
            IUser user = GetConnectedUserOrNull(authData.Value.Username);
            if (user != null)
            {
                user.Disconnect();
                _connectedUsers.Remove(user.Username, out var tuser);
            }
        }

        public Result Register(string token, MemberInfo memberInfo, string password)
        {
            if (GetUserIfConnectedOrLoggedIn(token).IsFailure)
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
            
            IUser newUser = new User(memberInfo.Clone());
            if (!_registeredUsersRepo.Add(newUser))
            {
                // TODO maybe remove the user form userAuth and log it
                return Result.Fail("User already exists");
            }

            return Result.Ok();
        }
        
        public Result RegisterAtAuthorization(string username, string password)
        {
            return _auth.Register(username, password);
        }
        
        public Result<string> Login(string guestToken, string username, string password, 
            ServiceUserRole role)
        {
            Result<AuthData> guestAuthData = _auth.GetDataIfConnected(guestToken);
            if (guestAuthData.IsFailure)
            {
                return Result.Fail<string>(guestAuthData.Error);;
            }
            
            Result<string> authLoginRes = _auth.Login(username, password, DtoUtils.ServiceUserRoleToAuthUserRole(role));
            if (authLoginRes.IsFailure)
            {
                return Result.Fail<string>(authLoginRes.Error);
            }
            
            IUser user = _registeredUsersRepo.GetOrNull(username);
            if (user == null)
            {
                // TODO log it since in auth the user can log in
                _auth.Logout(authLoginRes.Value);
                return Result.Fail<string>("Invalid username or password");
            }

            _connectedUsers.Remove(guestAuthData.Value.Username, out var guestUser);
            _connectedUsers.TryAdd(username, user);
            
            _auth.Disconnect(guestToken);
            return Result.Ok(authLoginRes.Value);
        }
        
        public Result<string> Logout(string token)
        {
            Result<AuthData> userAuthDataRes = _auth.GetDataIfLoggedIn(token);
            if (userAuthDataRes.IsFailure)
            {
                // TODO log it
                return Result.Fail<string>(userAuthDataRes.Error);
            }

            AuthData authData = userAuthDataRes.Value;
            IUser user = GetConnectedUserOrNull(authData.Username);
            if (user == null)
            {
                // TODO log it since in auth has the user logged in
                return Result.Fail<string>("Error in logout");
            }

            _auth.Logout(token);

            string newGuestToken = _auth.Connect();
            string newGuestUsername = _auth.GetDataIfConnected(newGuestToken).Value.Username;
            
            _connectedUsers.Remove(user.Username, out var tuser);
            _connectedUsers.TryAdd(newGuestUsername, new User(newGuestUsername));
            
            return Result.Ok(newGuestToken);
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
        
        /// <summary>
        /// Get the user if connected or logged in
        /// </summary>
        /// <param name="token">The Authorization token</param>
        /// <returns>The user recognized by the token</returns>
        public Result<IUser> GetUserIfConnectedOrLoggedIn(string token)
        {
            Result<AuthData> userAuthDataRes = _auth.GetDataIfLoggedIn(token);
            if (userAuthDataRes.IsFailure)
            {
                userAuthDataRes = _auth.GetDataIfConnected(token);
                if (userAuthDataRes.IsFailure)
                {
                    // TODO log it
                    return Result.Fail<IUser>(userAuthDataRes.Error);
                }
            }

            AuthData authData = userAuthDataRes.Value;
            IUser user = GetConnectedUserOrNull(authData.Username);
            if (user == null)
            {
                // TODO log it since in auth has the user logged in
                return Result.Fail<IUser>("Error with the connection of the user");
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
        
        private void ExchangeKeyToConnected(string fromUsername, string toUsername)
        {
            IUser user = GetConnectedUserOrNull(fromUsername);
            if (user == null)
            {
                return;
            }

            _connectedUsers.Remove(fromUsername, out var tuser);
            _connectedUsers.TryAdd(toUsername, user);
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

            if (!RegexUtils.IsValidEmail(memberInfo.Email))
            {
                return Result.Fail("Invalid email address");
            }

            return Result.Ok();
        }
    }
}