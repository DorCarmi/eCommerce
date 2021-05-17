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
    /// Calculate total price for basket
    /// </UC>
    /// <Req>
    /// 2.9
    /// </Req>
    /// </summary>
    //[TestFixture]
    //[Order(6)]
    public class TestCartPrice
    {
        private IAuthService _auth;
        private IStoreService _store;
        private ICartService _cart;
        private string store = "Yossi's Store";


        [SetUpAttribute]
        public void SetUp()
        {
            StoreRepository SR = new StoreRepository();
            TRegisteredUserRepo RP = new TRegisteredUserRepo();
            UserAuth UA = UserAuth.CreateInstanceForTests(RP);
            IRepository<IUser> UR = new RegisteredUsersRepository();

            _auth = AuthService.CreateUserServiceForTests(UA, UR, SR);
            _store = StoreService.CreateUserServiceForTests(UA, UR, SR);
            _cart = CartService.CreateUserServiceForTests(UA, UR, SR);
            MemberInfo yossi = new MemberInfo("Yossi11", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            string token = _auth.Connect();
            _auth.Register(token, yossi, "qwerty123");
            Result<string> yossiLogInResult = _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            IItem product = new SItem("Tara milk", store, 10, "dairy",
                new List<string> {"dairy", "milk", "Tara"}, (double) 5.4);
            _store.OpenStore(yossiLogInResult.Value, store);
            _store.AddNewItemToStore(yossiLogInResult.Value, product);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TearDownAttribute]
        public void Teardown()
        {
            _auth = null;
            _store = null;
            _cart = null;
        }

        [Test]
        public void TestEmpty()
        {
            string token = _auth.Connect();
            Result<double> result = _cart.GetPurchaseCartPrice(token);
            Assert.True(result.IsSuccess && result.Value == 0.0);
            _auth.Disconnect(token);
        }
        
        [Test]
        public void TestNonEmpty()
        {
            string token = _auth.Connect();
            _cart.AddItemToCart(token, "Tara milk", store, 3);
            Result<double> result = _cart.GetPurchaseCartPrice(token);
            Assert.True(result.IsSuccess && result.Value != 0.0);
            _auth.Disconnect(token);
        }
    }
}