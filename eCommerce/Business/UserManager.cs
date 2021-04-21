using System;
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
        private ConcurrentDictionary<string, IUser> _admins;

        private ConcurrentIdGenerator _concurrentIdGenerator;


        public UserManager(IUserAuth auth, IRepository<IUser> registeredUsersRepo)
        {
            _auth = auth;
            _connectedUsers = new ConcurrentDictionary<string, IUser>();
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
            return token;
        }
        
        public void Disconnect(string token)
        {
            _connectedUsers.Remove(token, out var userTuple);
            if (!_auth.IsValidToken(token))
            {
                // log it
                return;
            }
            
            if (userTuple == null)
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

            if (!_auth.IsValidToken(token))
            {
                // log it old token
                _connectedUsers.Remove(token, out var user);
                return Result.Fail("Invalid token");
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
            _admins.TryAdd(adminInfo.Username, user);
        }
        
        private Result RegisterAtAuthorization(string username, string password)
        {
            return _auth.Register(username, password);
        }
        
        public Result<string> Login(string guestToken, string username, string password, ServiceUserRole role)
        {
            if (!_auth.IsValidToken(guestToken))
            {
                _connectedUsers.Remove(guestToken, out var tuser);
                return Result.Fail<string>("Invalid token");
            }

            if (!_connectedUsers.TryGetValue(guestToken, out var guestUser) || guestUser.GetState() != Guest.State)
            {
                return Result.Fail<string>("Not connected or not guest");
            }
            
            Result authLoginRes = _auth.Authenticate(username, password);
            if (authLoginRes.IsFailure)
            {
                return Result.Fail<string>(authLoginRes.Error);
            }
            string loginToken = _auth.GenerateToken(username);
            
            IUser user = _registeredUsersRepo.GetOrNull(username);
            if (user == null)
            {
                return Result.Fail<string>("Invalid username or password");
            }

            // TODO check the role update method
            if (user.GetState() == Guest.State)
            {
                
            }
            
            _connectedUsers.Remove(guestToken, out var tguestUser);
            if (!_connectedUsers.TryAdd(loginToken, user))
            {
                //Error in token generation
                return Result.Fail<string>("Error");
            }
            
            return Result.Ok(loginToken);
        }
        
        public Result<string> Logout(string token)
        {
            if (!_auth.IsValidToken(token) || !_connectedUsers.TryGetValue(token, out IUser user))
            {
                // TODO log it
                return Result.Fail<string>("Invalid token");
            }

            if (user.GetState() == Guest.State)
            {
                return Result.Fail<string>("Guest can disconnect not to logout");
            }

            string newGuestToken = Connect();
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

        private long GetAndIncrementGuestId()
        {
            return _concurrentIdGenerator.MoveNext();
        }
        
        private string GenerateGuestUsername()
        {
            return $"_Guest{GetAndIncrementGuestId():D}";
        }
    }
}