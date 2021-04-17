using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using eCommerce.Auth;
using eCommerce.Common;

namespace Tests.Business
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

        private ConcurrentDictionary<string, string> _connectedGuests;
        private ConcurrentDictionary<string, AuthData> _registeredUsers;
        private ConcurrentDictionary<string, string> _loggedInUsers;

        private ConcurrentIdGenerator _token;

        public UserAuthMock()
        {
            _connectedGuests = new ConcurrentDictionary<string, string>();
            _registeredUsers = new ConcurrentDictionary<string, AuthData>();
            _loggedInUsers = new ConcurrentDictionary<string, string>();
            _token = new ConcurrentIdGenerator(0);
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
            if (IsLoggedIn(username))
            {
                return Result.Fail<string>("User already logged in");
            }

            string tokenStr = _token.MoveNext().ToString();
            _loggedInUsers.TryAdd(tokenStr, username);
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
            foreach (var user in _loggedInUsers.Values)
            {
                if(user.Equals(username))
                {
                    return true;
                }
            }

            return false;
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

            _loggedInUsers.TryGetValue(token, out var username);
            _registeredUsers.TryGetValue(username, out var userAuth);
            return Result.Ok(userAuth);
        }
    }
}