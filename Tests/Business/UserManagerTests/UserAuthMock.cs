using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using eCommerce.Auth;
using eCommerce.Common;

namespace Tests.Business.UserManagerTests
{
    /// <summary>
    /// <para>Implementation of concurrent.</para>
    /// <para>
    /// Assume Role in login is valid <br/>
    /// Assume password is valid <br/>
    /// Assume logout token is of login user
    /// </para>
    /// </summary>
    public class UserAuthMock : IUserAuth
    {

        // token to name
        private ConcurrentDictionary<string, string> _connectedGuests;
        // username to auth
        private ConcurrentDictionary<string, AuthData> _registeredUsers;
        //token to name
        private ConcurrentDictionary<string, string> _loggedInUsers;

        private ConcurrentIdGenerator _token;
        private Mutex _isLoginMutex;

        public UserAuthMock()
        {
            _connectedGuests = new ConcurrentDictionary<string, string>();
            _registeredUsers = new ConcurrentDictionary<string, AuthData>();
            _loggedInUsers = new ConcurrentDictionary<string, string>();
            _token = new ConcurrentIdGenerator(0);
            _isLoginMutex = new Mutex();
        }

        public string Connect()
        {
            string tokenStr = _token.MoveNext().ToString();
            _connectedGuests.TryAdd(tokenStr, $"Guest{tokenStr}");
            return tokenStr;
        }

        public void Disconnect(string token)
        {
            _connectedGuests.Remove(token, out var tstring );
        }

        public Result Register(string username, string password)
        {
            if (!_registeredUsers.TryAdd(username, new AuthData(username, "Member")))
            {
                return Result.Fail<string>("Username already exists");
            }

            return Result.Ok();
        }

        public Result<string> Login(string username, string password, AuthUserRole role)
        {
            _isLoginMutex.WaitOne();
            if (IsLoggedIn(username))
            {
                _isLoginMutex.ReleaseMutex();
                return Result.Fail<string>("User already logged in");
            }

            string tokenStr = _token.MoveNext().ToString();
            if (!_loggedInUsers.TryAdd(tokenStr, username))
            {
                _isLoginMutex.ReleaseMutex();
                return Result.Fail<string>("User already logged in");
            }
            _isLoginMutex.ReleaseMutex();

            return Result.Ok(tokenStr);
        }

        public Result Logout(string token)
        {
            _loggedInUsers.Remove(token, out var tstring );
            return Result.Ok();
        }

        public bool IsConnected(string token)
        {
            return _connectedGuests.ContainsKey(token);
        }

        public bool IsRegistered(string username)
        {
            return _registeredUsers.ContainsKey(username);
        }

        public bool IsLoggedIn(string username)
        {
            bool loggedIn = false;
            foreach (var user in _loggedInUsers.Values)
            {
                if(user.Equals(username))
                {
                    loggedIn = true;
                    break;
                }
            }
            return loggedIn;
        }

        public bool IsValidToken(string token)
        {
            return long.TryParse(token, out var tokenNumber);
        }

        public string RoleToString(AuthUserRole role)
        {
            return role.ToString();
        }

        public Result<AuthData> GetData(string token)
        {
            if (_connectedGuests.TryGetValue(token, out var guestName))
            {
                return Result.Ok(new AuthData(guestName, "Guest"));
            }

            if (!_loggedInUsers.TryGetValue(token, out var username))
            {
                return Result.Fail<AuthData>("Not logged in");
            }
            _registeredUsers.TryGetValue(username, out var userAuth);
            return Result.Ok(userAuth);
        }
    }
}