﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Auth;
using eCommerce.Common;
using NUnit.Framework;

namespace Tests.Auth
{
    
    [TestFixture]
    public class UserAuthTest
    {
        private IUserAuth _userAuth;
        private IDictionary<string, AuthData> _connectedGuest;
        private IList<TestsUserData> _registeredUsers;
        private IDictionary<string, TestsUserData> _loggedinUsers;

        public UserAuthTest()
        {
            _userAuth = UserAuth.CreateInstanceForTests(new TestsRegisteredUserRepo());
            _connectedGuest = new Dictionary<string, AuthData>(); 
            _registeredUsers = new List<TestsUserData>();
            _loggedinUsers = new Dictionary<string, TestsUserData>();
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
            IList<TestsUserData> userData = new List<TestsUserData>
            {
                new TestsUserData("User1", "User1"),
                new TestsUserData("TheGreatStore", "ThisIsTheGreatestStorePassword")
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
                TestsUserData user = _registeredUsers.First();
                Result registerRes = _userAuth.Register(user.Username, user.Password);
                Assert.True(registerRes.IsFailure,
                            $"Register already registered user: username: {user.Username} password: {user.Password}");
            }
        }
        
        [Test, Order(5)]
        public void RegisterInvalidUsersTest()
        {
            IList<TestsUserData> userData = new List<TestsUserData>
            {
                new TestsUserData(null, "User1"),
                new TestsUserData("TheGreatStore", null),
                new TestsUserData(null, null)
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
            IList<TestsUserData> usersToRemove = new List<TestsUserData>();
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
                string token = _userAuth.Connect();
                Result<string> loginRes = _userAuth.Login(token, user.Username, user.Password, UserRole.Member);
                Assert.True(loginRes.IsSuccess,
                            $"Registered user {user.Username} wasn't able to login\nError: {loginRes.Error}");
                
                Assert.True(_userAuth.IsLoggedIn(user.Username),
                    $"The user {user.Username} logged in but the system say it isn't");
                
                _loggedinUsers.Add(loginRes.Value, user);
            }
        }
        
        [OneTimeTearDown]
        public void TestsCleanup()
        {
            foreach (var loggedinUser in _loggedinUsers)
            {
                _connectedGuest.Add(_userAuth.Logout(loggedinUser.Key), null);
            }
            
            foreach (var guestToken in _connectedGuest.Keys)
            {
                _userAuth.Disconnect(guestToken);
            }
        }
        
    }
}