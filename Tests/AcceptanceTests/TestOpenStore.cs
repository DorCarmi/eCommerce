using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using eCommerce.Auth;
using eCommerce.Business;
using eCommerce.Common;
using eCommerce.Service;
using NUnit.Framework;
using Tests.AuthTests;

namespace Tests.AcceptanceTests
{
    /// <summary>
    /// <UC>
    /// Open a store
    /// </UC>
    /// <Req>
    /// 3.2
    /// </Req>
    /// </summary>
    [TestFixture]
    [Order(13)]
    public class TestOpenStore
    {
        private IAuthService _auth;
        private IStoreService _store;

        [SetUpAttribute]
        public void SetUp()
        {
            StoreRepository SR = new StoreRepository();
            TRegisteredUserRepo RP = new TRegisteredUserRepo();
            UserAuth UA = UserAuth.CreateInstanceForTests(RP);
            IRepository<IUser> UR = new RegisteredUsersRepository();

            _auth = AuthService.CreateUserServiceForTests(UA, UR, SR);
            _store = StoreService.CreateUserServiceForTests(UA, UR, SR);
            MemberInfo yossi = new MemberInfo("Mechanism100", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("PhroggyPal", "shiran@gmail.com", "Shiran Moris",
                DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            string token = _auth.Connect();
            _auth.Register(token, yossi, "qwerty123");
            _auth.Register(token, shiran, "130452abc");
            _auth.Disconnect(token);
        }
        
        [TearDownAttribute]
        public void Teardown()
        {
            _auth = null;
            _store = null;
        }
        
        
        [TestCase("Mechanism100", "qwerty123","aldi")]
        [TestCase("PhroggyPal", "130452abc", "walmart")]
        [Order(0)]
        [Test]
        public void TestOpenStoreSuccess(string member, string password, string store)
        {
            string token = _auth.Connect();
            Result<string> login = _auth.Login(token, member, password, ServiceUserRole.Member);
            Result result = _store.OpenStore(login.Value, store);
            Assert.True(result.IsSuccess, result.Error);
            _auth.Logout(login.Value);
            _auth.Disconnect(token);
        }
        
        [TestCase("Mechanism100", "qwerty123","Yossi's store2022", "PhroggyPal", "130452abc", "dancing dragon2022")]
        [TestCase("Mechanism100", "qwerty123","Yossi's store2021", "Mechanism100", "qwerty123", "dancing dragon2021")]
        [Order(1)]
        public void TestOpenStoreMultipleSuccess(string firstMember, string firstPassword, string firstStore, string secondMember, string secondPassword, string secondStore)
        {
            string token = _auth.Connect();
            Result<string> login = _auth.Login(token, firstMember, firstPassword, ServiceUserRole.Member);
            Result result = _store.OpenStore(login.Value, firstStore);
            Assert.True(result.IsSuccess, result.Error);
            token = _auth.Logout(login.Value).Value;
            login = _auth.Login(token, secondMember, secondPassword, ServiceUserRole.Member);
            result = _store.OpenStore(login.Value, secondStore);
            Assert.True(result.IsSuccess, result.Error);
            token = _auth.Logout(login.Value).Value;
            _auth.Disconnect(token);
        }
        
        
        [TestCase("Mechanism100", "qwerty123","Yossi's store","Yossi's store")]
        [Test]
        public void TestOpenStoreFailureInput(string member, string password, string storeName, string itemStore)
        { 
            string token = _auth.Connect();
            Result<string> login = _auth.Login(token, member, password, ServiceUserRole.Member);
            Result result = _store.OpenStore(login.Value, storeName);
            Assert.True(result.IsFailure, "store opening was suppose to fail");
            _auth.Logout(login.Value);
            _auth.Disconnect(token);
        }
        
        [TestCase("Mechanism100", "qwerty123","Yossi's store", "PhroggyPal", "130452abc")]
        [TestCase("Mechanism100", "qwerty123","Yossi's store", "Mechanism100", "qwerty123")]
        public void TestOpenStoreFailureAlreadyExists(string firstMember, string firstPassword, string store, string secondMember, string secondPassword)
        {
            string token = _auth.Connect();
            Result<string> login = _auth.Login(token, firstMember, firstPassword, ServiceUserRole.Member);
            _store.OpenStore(login.Value, store);
            token = _auth.Logout(login.Value).Value;
            login = _auth.Login(token, secondMember, secondPassword, ServiceUserRole.Member);
            Result result = _store.OpenStore(login.Value, store);
            Assert.True(result.IsFailure, "store name already used. expected fail");
            token = _auth.Logout(login.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("prancing dragon")]
        [TestCase("Yossi's store")]
        [Test]
        public void TestOpenStoreFailureNotLoggedIn(string storeName)
        { 
            string token = _auth.Connect();
            Result result = _store.OpenStore(token, storeName);
            Assert.True(result.IsFailure, "store opening was suppose to fail, user not logged in");
            _auth.Disconnect(token);
        }
        
    }
}