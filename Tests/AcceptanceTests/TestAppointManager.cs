using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using eCommerce.Business;
using eCommerce.Common;
using eCommerce.Service;
using NUnit.Framework;

namespace Tests.AcceptanceTests
{
    /// <summary>
    /// <UC>
    /// Appoint manager
    /// </UC>
    /// <Req>
    /// 4.5
    /// </Req>
    /// </summary>
    [TestFixture]
    [Order(1)]
    public class TestAppointManager
    {
        private IAuthService _auth;
        private IStoreService _store;
        private IUserService _user;
        private string store = "Yossi's Store";
        
        
        [SetUp]
        public void SetUp()
        {
            _auth = new AuthService();
            _store = new StoreService();
            _user = new UserService();
            MemberInfo yossi = new MemberInfo("Yossi11", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("singerMermaid", "shiran@gmail.com", "Shiran Moris",
                DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            MemberInfo lior = new MemberInfo("Liorwork","lior@gmail.com", "Lior Lee", 
                DateTime.ParseExact("05/07/1996", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Carl Neter 14");
            string token = _auth.Connect();
            _auth.Register(token, yossi, "qwerty123");
            _auth.Register(token, shiran, "130452abc");
            _auth.Register(token, lior, "987654321");
            Result<string> yossiLogInResult = _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            _store.OpenStore(yossiLogInResult.Value, store);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        
        [TestCase("Yossi's Store", "singerMermaid")]
        [TestCase("Yossi's Store", "Liorwork")]
        [Order(1)]
        [Test]
        public void TestSuccess(string storeName, string username)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result result = _user.AppointManager(yossiLogin.Value, storeName, username);
            Assert.True(result.IsSuccess, "failed to appoint " + username + ": " + result.Error);
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("Yossi's Store", "singerMermaid")]
        [TestCase("Yossi's Store", "Liorwork")]
        [Test]
        public void TestFailureDouble(string storeName, string username)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            _user.AppointManager(yossiLogin.Value, storeName, username);
            Result result = _user.AppointManager(yossiLogin.Value, storeName, username);
            Assert.True(result.IsFailure, "Appointing manager was suppose to fail, manager already appointed");
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token);
        }

        [TestCase("Yossi11", "qwerty123", "The Polite Frog", "singerMermaid")]
        [TestCase("Yossi11",  "qwerty123", "Yossi's Store", "Yossi11")] 
        [TestCase("Yossi11",   "qwerty123", "Yossi's Store", "Tamir123")]
        [TestCase("singerMermaid", "130452abc", "Yossi's Store", "Liorwork")]
        [Test]
        public void TestFailureInvalid(string appointer, string appointerPassword,  string storeName, string username)
        {
            string token = _auth.Connect();
            Result<string>login = _auth.Login(token, appointer, appointerPassword, ServiceUserRole.Member);
            Result result = _user.AppointManager(login.Value, storeName, username);
            Assert.True(result.IsFailure, "Appointing " + username + " was expected to fail!");
            token = _auth.Logout(login.Value).Value;
            _auth.Disconnect(token);
        }
        
        
        [TestCase("Yossi's Store", "singerMermaid")]
        [TestCase("Yossi's Store", "Liorwork")]
        [Test]
        public void TestFailureLogic(string storeName, string username)
        {
            string token = _auth.Connect();
            Result result = _user.AppointManager(token, storeName, username);
            Assert.True(result.IsFailure, "Appointing " + username + " was expected to fail since the user wasn't logged in!");
            _auth.Disconnect(token);
        }
    }
}