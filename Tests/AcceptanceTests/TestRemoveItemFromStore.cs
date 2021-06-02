using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using eCommerce.Auth;
using eCommerce.Business;
using eCommerce.Business.Repositories;
using eCommerce.Common;
using eCommerce.Service;
using NUnit.Framework;

namespace Tests.AcceptanceTests
{
    /// <summary>
    /// <UC>
    /// Remove product from store
    /// </UC>
    /// <Req>
    /// 4.1
    /// </Req>
    /// </summary>
    [TestFixture]
    [Order(14)]
    public class TestRemoveItemFromStore
    {
        private IAuthService _auth;
        private INStoreService _inStore;
        private string storeName = "Target";

        [SetUpAttribute]
        public async Task SetUp()
        {
            StoreRepository SR = new StoreRepository();
            InMemoryRegisteredUserRepo RP = new InMemoryRegisteredUserRepo();
            UserAuth UA = UserAuth.CreateInstanceForTests(RP);
            IRepository<User> UR = new InMemoryRegisteredUsersRepository();

            _auth = AuthService.CreateUserServiceForTests(UA, UR, SR);
            _inStore = InStoreService.CreateUserServiceForTests(UA, UR, SR);
            MemberInfo yossi = new MemberInfo("Mechanism1000", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("PhrogLiv", "shiran@gmail.com", "Shiran Moris",
                DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            string token = _auth.Connect();
            await _auth.Register(token, yossi, "qwerty123");
            await _auth.Register(token, shiran, "130452abc");
            Result<string> yossiLogInResult = await _auth.Login(token, "Mechanism1000", "qwerty123", ServiceUserRole.Member);
            IItem product = new SItem("Tara milk", storeName, 10, "dairy",
                new List<string>{"dairy", "milk", "Tara"}, (double)5.4);
            _inStore.OpenStore(yossiLogInResult.Value, storeName);
            _inStore.AddNewItemToStore(yossiLogInResult.Value, product);
            _inStore.AddNewItemToStore(yossiLogInResult.Value, new SItem("iPhone X", storeName, 35, "smartphones", 
                new List<string>{"smartphone", "iPhone", "Apple", "Iphone X"}, (double) 5000.99));
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TearDownAttribute]
        public void Teardown()
        {
            _auth = null;
            _inStore = null;
        }
        
        [TestCase("Tara milk")]
        [TestCase("iPhone X")]
        [Order(0)]
        [Test]
        public async Task TestRemoveItemFromStoreSuccess(string productName)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = await _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result removeItemResult = _inStore.RemoveItemFromStore(yossiLogin.Value, storeName, productName);
            Assert.True(removeItemResult.IsSuccess, "failed to remove item " + productName + ": " + removeItemResult.Error);
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token); 
        }
        
        [TestCase("Target", "Gans 356 air", "Mechanism1000", "qwerty123")]
        [TestCase("dancing doors", "Tara milk", "Mechanism1000", "qwerty123")] // non existing store
        [TestCase("Target", "Gans 356 air", "PhrogLiv", "130452abc")]
        [Test]      
        public async Task TestRemoveItemFromStoreFailureInvalid(string store, string productName, string member, string password)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = await _auth.Login(token, member, password, ServiceUserRole.Member);
            Result removeItemResult = _inStore.RemoveItemFromStore(yossiLogin.Value, store, productName);
            Assert.True(removeItemResult.IsFailure);
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token); 
        }

       
        
        [TestCase("Tara milk")]
        [TestCase("iPhone X")]
        [Test]
        public void TestRemoveItemFromStoreFailureLogic(string productName)
        {
            string token = _auth.Connect();
            Result removeItemResult = _inStore.RemoveItemFromStore(token, storeName, productName);
            Assert.True(removeItemResult.IsFailure, "Suppose to fail, user not logged in");
            _auth.Disconnect(token); 
        }
    }
}