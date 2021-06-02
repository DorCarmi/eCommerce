using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using eCommerce.Auth;
using System.Threading.Tasks;
using eCommerce.Business;
using eCommerce.Common;
using eCommerce.Service;
using NUnit.Framework;
using Tests.AuthTests;

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
    [Order(2)]
    public class TestAdminRequestsHistory
    {
        private IAuthService _auth;
        private IUserService _user;
        private IStoreService _store;
        
        [SetUpAttribute]
        public async Task SetUp()
        {
            InMemoryRegisteredUserRepo RP = new InMemoryRegisteredUserRepo();
            UserAuth UA = UserAuth.CreateInstanceForTests(RP);
            StoreRepository SR = new StoreRepository();
            IRepository<IUser> UR = new RegisteredUsersRepository();

            _auth = AuthService.CreateUserServiceForTests(UA, UR, SR);
            _store = StoreService.CreateUserServiceForTests(UA, UR, SR);
            _user = UserService.CreateUserServiceForTests(UA, UR, SR);
            MemberInfo yossi = new MemberInfo("Yossi11","yossi@gmail.com", "Yossi Park", DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("singerMermaid","shiran@gmail.com", "Shiran Moris", DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            MemberInfo lior = new MemberInfo("Liorwork","lior@gmail.com", "Lior Lee", DateTime.ParseExact("05/07/1996", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Carl Neter 14");
            string token = _auth.Connect();
            await _auth.Register(token, yossi, "qwerty123");
            await _auth.Register(token, shiran, "130452abc");
            await _auth.Register(token, lior, "987654321");
            Result<string> yossiLogInResult = await _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            string storeName = "Yossi's Store";
            IItem product = new SItem("Tara milk", storeName, 10, "dairy",
                new List<string>{"dairy", "milk", "Tara"}, (double)5.4);
            _store.OpenStore(yossiLogInResult.Value, storeName);
            _store.AddNewItemToStore(yossiLogInResult.Value, product);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TearDownAttribute]
        public void Teardown()
        {
            _auth = null;
            _store = null;
            _user = null;
        }
        
        [TestCase("Yossi11")]
        [TestCase("singerMermaid")] 
        [Test]
        public async Task TestAdminUserSuccess(string member)
        { 
            string token = _auth.Connect();
            Result<string> login = await _auth.Login(token, "_Admin", "_Admin", ServiceUserRole.Admin);
            Result result = _user.AdminGetPurchaseHistoryUser(login.Value,member);
            Assert.True(result.IsSuccess, result.Error);
            _auth.Logout(login.Value);
            _auth.Disconnect(token);
        }
        
        [TestCase("Tamir")]
        [TestCase("++uhs++")] 
        [Test]
        public async Task TestAdminUserFailure(string member)
        { 
            string token = _auth.Connect();
            Result<string> login = await _auth.Login(token, "_Admin", "_Admin", ServiceUserRole.Admin);
            Result result = _user.AdminGetPurchaseHistoryUser(login.Value,member);
            Assert.True(result.IsFailure);
            _auth.Logout(login.Value);
            _auth.Disconnect(token);
        }
       
        [TestCase("Yossi11", "qwerty123", "singerMermaid")]
        [TestCase("singerMermaid", "130452abc", "Yossi11")] 
        [Test]
        public async Task TestAdminUserFailureLogin(string admin, string password,string member)
        { 
            string token = _auth.Connect();
            Result<string> login = await _auth.Login(token, admin, password, ServiceUserRole.Admin);
            Result result = _user.AdminGetPurchaseHistoryUser(login.Value,member);
            Assert.True(result.IsFailure);
            _auth.Logout(login.Value);
            _auth.Disconnect(token);
        }
       
        [TestCase("Yossi's Store")] 
        [Test]
        public async Task TestAdminStoreSuccess(string storeID)
        { 
            string token = _auth.Connect();
            Result<string> login = await _auth.Login(token, "_Admin", "_Admin", ServiceUserRole.Admin);
            Result result = _user.AdminGetPurchaseHistoryStore(login.Value,storeID);
            Assert.True(result.IsSuccess, result.Error);
            _auth.Logout(login.Value);
            _auth.Disconnect(token);
        }
        [TestCase("dancing dragon")]       
        [Test]
        public async Task TestAdminStoreFailure(string storeID)
        { 
            string token = _auth.Connect();
            Result<string> login = await _auth.Login(token, "_Admin", "_Admin", ServiceUserRole.Admin);
            Result result = _user.AdminGetPurchaseHistoryStore(login.Value,storeID);
            Assert.True(result.IsFailure);
            _auth.Logout(login.Value);
            _auth.Disconnect(token);
        }
        
        [TestCase("singerMermaid", "130452abc", "Prancing dragon")] 
        [Order(0)]
        [Test]
        public async Task TestAdminStoreFailureLogin(string admin, string password,string storeId)
        { 
            string token = _auth.Connect();
            Result<string> login = await _auth.Login(token, admin, password, ServiceUserRole.Admin);
            Result result = _user.AdminGetPurchaseHistoryStore(login.Value,storeId);
            Assert.True(result.IsFailure);
            _auth.Logout(login.Value);
            _auth.Disconnect(token);
        }
    }
}