using System;
using System.Collections;
using System.Collections.Generic;
using eCommerce.Auth;
using eCommerce.Common;
using NUnit.Framework;

namespace Tests.Auth
{
    
    [TestFixture]
    public class UserAuthTest
    {
        private IUserAuth _userAuth;
        private IList<string> _connectedGuestTokens;
        private IList<TestsUserData> _registeredUsers;

        public UserAuthTest()
        {
            _userAuth = UserAuth.CreateInstanceForTests(new TestsRegisteredUserRepo());
            _connectedGuestTokens = new List<string>();
            _registeredUsers = new List<TestsUserData>();
        }

        [Test]
        public void ConnectTest()
        {
            Result<string> tokenRes = _userAuth.Connect();

            Assert.True(tokenRes.IsSuccess,
                        "Error when generating the guest token");

            Result<AuthData> authDataRes = _userAuth.GetData(tokenRes.Value);
            Assert.True(authDataRes.IsSuccess,
                        "Generated token from Connect is not valid");
            
            Assert.AreEqual("Guest",
                            authDataRes.Value.Role,
                            "Token generated for the guest doesnt not have the matching role");
            Console.WriteLine(authDataRes.Value.Username);
            _connectedGuestTokens.Add(tokenRes.Value);
        }

        [Test]
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
                            $"User {user.Username} want registered, Result Message: {registerRes.Error}");
                _registeredUsers.Add(user);
            }
        }
        
        [Test]
        public void IsUserRegisteredTest()
        {
            foreach (var user in _registeredUsers)
            {
                Assert.True(_userAuth.IsRegistered(user.Username),
                    $"User {user.Username} want registered, but test RegisterValidUsersTest said it was");
                _registeredUsers.Add(user);
            }
        }
        
        [OneTimeTearDown]
        public void TestsCleanup()
        {
            foreach (var guestToken in _connectedGuestTokens)
            {
                _userAuth.Disconnect(guestToken);
            }
        }
        
    }
}