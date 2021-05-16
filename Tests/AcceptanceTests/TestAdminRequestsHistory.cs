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
    /// Admin requests for user’s Purchase history
    /// Admin requests for store history 
    /// </UC>
    /// <Req>
    /// 6.4
    /// </Req>
    /// </summary>
    [TestFixture]
    public class TestAdminRequestsHistory
    {
        private IAuthService _auth;
        private IUserService _user;
        private IStoreService _store;
        
        [SetUp]
        public void SetUp()
        {
            _auth = new AuthService();
            _user = new UserService();
            _store = new StoreService();
            MemberInfo yossi = new MemberInfo("Yossi11","yossi@gmail.com", "Yossi Park", DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("singerMermaid","shiran@gmail.com", "Shiran Moris", DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            MemberInfo lior = new MemberInfo("Liorwork","lior@gmail.com", "Lior Lee", DateTime.ParseExact("05/07/1996", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Carl Neter 14");
            string token = _auth.Connect();
            _auth.Register(token, yossi, "qwerty123");
            _auth.Register(token, shiran, "130452abc");
            _auth.Register(token, lior, "987654321");
            Result<string> yossiLogInResult = _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            string storeName = "Yossi's Store";
            IItem product = new SItem("Tara milk", storeName, 10, "dairy",
                new List<string>{"dairy", "milk", "Tara"}, (double)5.4);
            _store.OpenStore(yossiLogInResult.Value, storeName);
            _store.AddNewItemToStore(yossiLogInResult.Value, product);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("Yossi11")]
        [TestCase("singerMermaid")] 
        [Test]
        public void TestAdminUserSuccess(string member)
        { 
            string token = _auth.Connect();
            Result<string> login = _auth.Login(token, "_Admin", "_Admin", ServiceUserRole.Admin);
            Result result = _user.AdminGetPurchaseHistoryUser(login.Value,member);
            Assert.True(result.IsSuccess, result.Error);
            _auth.Logout(login.Value);
            _auth.Disconnect(token);
        }
        
        [TestCase("Tamir")]
        [TestCase("++uhs++")] 
        [Test]
        public void TestAdminUserFailure(string member)
        { 
            string token = _auth.Connect();
            Result<string> login = _auth.Login(token, "_Admin", "_Admin", ServiceUserRole.Admin);
            Result result = _user.AdminGetPurchaseHistoryUser(login.Value,member);
            Assert.True(result.IsFailure);
            _auth.Logout(login.Value);
            _auth.Disconnect(token);
        }
       
        [TestCase("Yossi11", "qwerty123", "singerMermaid")]
        [TestCase("singerMermaid", "130452abc", "Yossi11")] 
        [Test]
        public void TestAdminUserFailureLogin(string admin, string password,string member)
        { 
            string token = _auth.Connect();
            Result<string> login = _auth.Login(token, admin, password, ServiceUserRole.Admin);
            Result result = _user.AdminGetPurchaseHistoryUser(login.Value,member);
            Assert.True(result.IsFailure);
            _auth.Logout(login.Value);
            _auth.Disconnect(token);
        }
       
        [TestCase("Yossi's Store")] 
        [Test]
        public void TestAdminStoreSuccess(string storeID)
        { 
            string token = _auth.Connect();
            Result<string> login = _auth.Login(token, "_Admin", "_Admin", ServiceUserRole.Admin);
            Result result = _user.AdminGetPurchaseHistoryStore(login.Value,storeID);
            Assert.True(result.IsSuccess, result.Error);
            _auth.Logout(login.Value);
            _auth.Disconnect(token);
        }
        [TestCase("dancing dragon")]       
        [Test]
        public void TestAdminStoreFailure(string storeID)
        { 
            string token = _auth.Connect();
            Result<string> login = _auth.Login(token, "_Admin", "_Admin", ServiceUserRole.Admin);
            Result result = _user.AdminGetPurchaseHistoryStore(login.Value,storeID);
            Assert.True(result.IsFailure);
            _auth.Logout(login.Value);
            _auth.Disconnect(token);
        }
        
        [TestCase("singerMermaid", "130452abc", "Prancing dragon")] 
        [Order(0)]
        [Test]
        public void TestAdminStoreFailureLogin(string admin, string password,string storeId)
        { 
            string token = _auth.Connect();
            Result<string> login = _auth.Login(token, admin, password, ServiceUserRole.Admin);
            Result result = _user.AdminGetPurchaseHistoryStore(login.Value,storeId);
            Assert.True(result.IsFailure);
            _auth.Logout(login.Value);
            _auth.Disconnect(token);
        }
    }
}