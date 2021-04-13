using System;
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

        public UserAuthTest()
        {
            _userAuth = UserAuth.GetInstance();
            _connectedGuestTokens = new List<string>();
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