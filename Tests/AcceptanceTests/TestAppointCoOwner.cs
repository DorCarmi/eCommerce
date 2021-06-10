using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using eCommerce.Auth;
using System.Threading.Tasks;
using eCommerce.Business;
using eCommerce.Business.Repositories;
using eCommerce.Common;
using eCommerce.Service;
using NuGet.Frameworks;
using NUnit.Framework;
using Tests.AuthTests;

namespace Tests.AcceptanceTests
{
    /// <summary>
    /// <UC>
    /// Appoint user to be store co-owner 
    /// </UC>
    /// <Req>
    /// 4.3
    /// </Req>
    /// </summary>
    [TestFixture]
    [Order(3)]
    public class TestAppointCoOwner
    {
        private IAuthService _auth;
        private INStoreService _inStore;
        private IUserService _user;
        private string store = "Yossi's Store";
        
        
        [SetUpAttribute]
        public async Task SetUp()
        {
            InMemoryRegisteredUserRepo RP = new InMemoryRegisteredUserRepo();
            UserAuth UA = UserAuth.CreateInstanceForTests(RP, "ThisKeyIsForTests");
            InMemoryStoreRepo SR = new InMemoryStoreRepo();
            IRepository<User> UR = new InMemoryRegisteredUsersRepository();
            IMarketFacade marketFacade = MarketFacade.CreateInstanceForTests(UA,UR, SR);

            _auth = AuthService.CreateUserServiceForTests(marketFacade);
            _inStore = InStoreService.CreateUserServiceForTests(marketFacade);
            _user = UserService.CreateUserServiceForTests(marketFacade);
            MemberInfo yossi = new MemberInfo("Yossi11", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("singerMerm", "shiran@gmail.com", "Shiran Moris",
                DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            MemberInfo lior = new MemberInfo("Lior","lior@gmail.com", "Lior Lee", 
                DateTime.ParseExact("05/07/1996", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Carl Neter 14");
            string token = _auth.Connect();
            await _auth.Register(token, yossi, "qwerty123");
            await _auth.Register(token, shiran, "130452abc");
            await _auth.Register(token, lior, "987654321");
            Result<string> yossiLogInResult = await _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            _inStore.OpenStore(yossiLogInResult.Value, store);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        [TearDownAttribute]
        public void Teardown()
        {
            _auth = null;
            _inStore = null;
            _user = null;
        }
        
        
        [TestCase("Yossi's Store", "singerMerm")]
        [TestCase("Yossi's Store", "Lior")]
        [Order(0)]
        [Test]
        public async Task TestAppointCoOwnerSuccess(string storeName, string username)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = await _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Assert.True(yossiLogin.IsSuccess, yossiLogin.Error);
            Result result = _user.AppointCoOwner(yossiLogin.Value, storeName, username);
            Assert.True(result.IsSuccess, "failed to appoint " + username + ": " + result.Error);
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("Yossi's Store", "singerMerm")]
        [TestCase("Yossi's Store", "Lior")]
        [Test]
        public async Task  TestAppointCoOwnerFailureDouble(string storeName, string username)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = await _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            _user.AppointCoOwner(yossiLogin.Value, storeName, username);
            Result result = _user.AppointCoOwner(yossiLogin.Value, storeName, username);
            Assert.True(result.IsFailure, "Appointing Co-Owner was suppose to fail, co-owner already appointed");
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token);
        }
        
        
        [TestCase("Yossi11", "qwerty123", "The Polite Frog", "singerMerm")]
        [TestCase("Yossi11",  "qwerty123", "Yossi's Store", "Yossi11")] 
        [TestCase("Yossi11",   "qwerty123", "Yossi's Store", "Tamir123")]
        [TestCase("singerMerm", "130452abc", "Yossi's Store", "Lior")]
        [Test]
        public async Task  TestAppointCoOwnerFailureInvalid(string appointer, string appointerPassword,  string storeName, string username)
        {
            string token = _auth.Connect();
            Result<string>login = await _auth.Login(token, appointer, appointerPassword, ServiceUserRole.Member);
            Result result = _user.AppointCoOwner(login.Value, storeName, username);
            Assert.True(result.IsFailure, "Appointing " + username + " was expected to fail!");
            token = _auth.Logout(login.Value).Value;
            _auth.Disconnect(token);
        }
        
        
        [TestCase("Yossi's Store", "singerMerm")]
        [TestCase("Yossi's Store", "Lior")]
        [Test]
        public void TestAppointCoOwnerFailureLogic(string storeName, string username)
        {
            string token = _auth.Connect();
            Result result = _user.AppointCoOwner(token, storeName, username);
            Assert.True(result.IsFailure, "Appointing " + username + " was expected to fail since the user wasn't logged in!");
            _auth.Disconnect(token);
        }

        [Test]
        public void Test_ABC_Test()
        {
            string tokenAb = _auth.Connect();
            string tokenBond = _auth.Connect();
            string tokenChum = _auth.Connect();

            string THE_PASSWORD = "qwerty123";
            string STORE_NAME = "AbiAndHisSons";
            
            
            MemberInfo Abraham = new MemberInfo("Abraham", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo Bond = new MemberInfo("Bond", "shiran@gmail.com", "Shiran Moris",
                DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            MemberInfo Chumacher = new MemberInfo("Chumacher","lior@gmail.com", "Lior Lee", 
                DateTime.ParseExact("05/07/1996", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Carl Neter 14");
            
            var abTask=_auth.Register(tokenAb, Abraham, THE_PASSWORD);
            var bondTask=_auth.Register(tokenBond, Bond, THE_PASSWORD);
            var chumTask=_auth.Register(tokenChum, Chumacher, THE_PASSWORD);
            
            Assert.True(abTask.Result.IsSuccess,abTask.Result.Error);
            Assert.True(bondTask.Result.IsSuccess,bondTask.Result.Error);
            Assert.True(chumTask.Result.IsSuccess,chumTask.Result.Error);
            
            //A->B
            var abrahamLogInTask = _auth.Login(tokenAb, Abraham.Username, THE_PASSWORD, ServiceUserRole.Member);
            var abrahamLogInResult = abrahamLogInTask.Result;
            Assert.True(abrahamLogInResult.IsSuccess,abrahamLogInResult.Error);
            var abiToken = abrahamLogInResult.Value;
            _inStore.OpenStore(abiToken, STORE_NAME);
            var appointBondRes=_user.AppointCoOwner(abiToken, STORE_NAME, Bond.Username);
            Assert.True(appointBondRes.IsSuccess,appointBondRes.Error);
            
            //B->C
            var bondLogInTask = _auth.Login(tokenBond, Bond.Username, THE_PASSWORD, ServiceUserRole.Member);
            var bondLogInResult = bondLogInTask.Result;
            Assert.True(bondLogInResult.IsSuccess,bondLogInResult.Error);
            var bondToken = bondLogInResult.Value;
            var appointChumRes=_user.AppointCoOwner(bondToken, STORE_NAME, Chumacher.Username);
            Assert.True(appointChumRes.IsSuccess,appointChumRes.Error);
            
            
            //C->
            var chumLogInTask = _auth.Login(tokenChum, Chumacher.Username, THE_PASSWORD, ServiceUserRole.Member);
            var chumLogInResult = chumLogInTask.Result;
            Assert.True(chumLogInResult.IsSuccess,chumLogInResult.Error);
            var chumToken = chumLogInResult.Value;


            //A-x->B->x->C
            var resRemoveOwner=_user.RemoveCoOwner(abiToken, STORE_NAME, Bond.Username);

            var permissionResAb = _inStore.GetStorePermission(abiToken, STORE_NAME);
            var permissionResBond = _inStore.GetStorePermission(bondToken, STORE_NAME);
            var permissionResChum = _inStore.GetStorePermission(chumToken, STORE_NAME);
            
            Assert.True(permissionResAb.IsSuccess,permissionResAb.Error);
            Assert.False(permissionResBond.IsSuccess);
            Assert.False(permissionResChum.IsSuccess);


        }
    }
}