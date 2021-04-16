using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Auth;
using eCommerce.Common;
using NUnit.Framework;

namespace Tests.AuthTests
{
    
    [TestFixture]
    public class UserAuthTests
    {
        private IUserAuth _userAuth;
        private IDictionary<string, AuthData> _connectedGuest;
        private IList<TUserData> _registeredUsers;
        private IDictionary<string, TUserData> _loggedinUsers;

        public UserAuthTests()
        {
            _userAuth = UserAuth.CreateInstanceForTests(new TRegisteredUserRepo());
            _connectedGuest = new Dictionary<string, AuthData>(); 
            _registeredUsers = new List<TUserData>();
            _loggedinUsers = new Dictionary<string, TUserData>();
        }

        [Test, Order(1)]
        public void ConnectTest()
        {
            for (int i = 0; i < 3; i++)
            {
                string token = _userAuth.Connect();

                Result<AuthData> authDataRes = _userAuth.GetData(token);
                Assert.True(authDataRes.IsSuccess,
                    "Generated token from Connect is not valid");

                AuthData authData = authDataRes.Value;
                Assert.AreEqual("Guest",
                    authData.Role,
                    "Token generated for the guest doesnt not have the matching role");

                Console.WriteLine($"Created guest name {authData.Username}");
                _connectedGuest.Add(token, authData);
            }
        }
        
        [Test, Order(2)]
        public void UniqueGuestNameTest()
        {
            ISet<string> nameSet = new HashSet<string>();
            foreach (var authData in _connectedGuest.Values)
            {
                Assert.True(nameSet.Add(authData.Username),
                    $"Duplicate guest name {authData.Username}");
            }
        }

        [Test, Order(3)]
        public void RegisterValidUsersTest()
        {
            IList<Result> registeredRes = new List<Result>();
            IList<TUserData> userData = new List<TUserData>
            {
                new TUserData("User1", "User1"),
                new TUserData("TheGreatStore", "ThisIsTheGreatestStorePassword")
            };

            foreach (var user in userData)
            {
                Result registerRes = _userAuth.Register(user.Username, user.Password);
                Assert.True(registerRes.IsSuccess,
                    $"User {user.Username} wasn't registered, Result Message: {registerRes.Error}");
                _registeredUsers.Add(user);
            }
        }

        [Test, Order(4)]
        public void TryRegistersRegisteredUsersTest()
        {
            if (_registeredUsers.Count > 0)
            {
                TUserData user = _registeredUsers.First();
                Result registerRes = _userAuth.Register(user.Username, user.Password);
                Assert.True(registerRes.IsFailure,
                            $"Register already registered user: username: {user.Username} password: {user.Password}");
            }
        }
        
        [Test, Order(5)]
        public void RegisterInvalidUsersTest()
        {
            IList<TUserData> userData = new List<TUserData>
            {
                new TUserData(null, "User1"),
                new TUserData("TheGreatStore", null),
                new TUserData(null, null)
            };

            foreach (var user in userData)
            {
                Result registerRes = _userAuth.Register(user.Username, user.Password);
                Assert.True(registerRes.IsFailure,
                    $"User {user.Username} with password {user.Password} was able to be registered, Result Message: {registerRes.Error}");
            }
        }
        
        [Test, Order(6)]
        public void IsUserRegisteredTest()
        {
            IList<TUserData> usersToRemove = new List<TUserData>();
            foreach (var user in _registeredUsers)
            {
                Assert.True(_userAuth.IsRegistered(user.Username),
                    $"User {user.Username} wasn't registered, but test RegisterValidUsersTest said it was");
                usersToRemove.Add(user);
            }

            foreach (var userToRemove in usersToRemove)
            {
                _registeredUsers.Remove(userToRemove);
            }
        }

        [Test, Order(7)]
        public void LogInAllRegisteredUsersTest()
        {
            foreach (var user in _registeredUsers)
            {
                Result<string> loginRes = _userAuth.Login(user.Username, user.Password, AuthUserRole.Member);
                Assert.True(loginRes.IsSuccess,
                            $"Registered user {user.Username} wasn't able to login\nError: {loginRes.Error}");

                _loggedinUsers.Add(loginRes.Value, user);
            }
        }
        
        [Test, Order(8)]
        public void LogoutAllTheLogInUsersTest()
        {
            foreach (var loggedinUser in _loggedinUsers)
            {
                Result logoutRes = _userAuth.Logout(loggedinUser.Key);
                Assert.True(logoutRes.IsSuccess,
                    $"The user {loggedinUser.Value.Username} was logged in but the logout say it wasn't");
            }
        }
        
        [OneTimeTearDown]
        public void TestsCleanup()
        {
            
            foreach (var loggedinUser in _loggedinUsers)
            {
                Result logoutRes = _userAuth.Logout(loggedinUser.Key);
            }
            
            foreach (var guestToken in _connectedGuest.Keys)
            {
                _userAuth.Disconnect(guestToken);
            }
        }
        
    }
}