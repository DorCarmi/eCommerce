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
    [TestFixture]
    public class TestBuyCart
    {
        private IAuthService _auth;
        private IStoreService _store;
        private ICartService _cart;
        private string store = "Yossi's Store";


        [SetUp]
        public void SetUp()
        {
            _auth = new AuthService();
            _store = new StoreService();
            _cart = new CartService();
            MemberInfo yossi = new MemberInfo("Yossi11", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            string token = _auth.Connect();
            _auth.Register(token, yossi, "qwerty123");
            Result<string> yossiLogInResult = _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            IItem product = new SItem("Tara milk", store, 10, "dairy",
                new ReadOnlyCollection<string>(new List<string> {"dairy", "milk", "Tara"}), (double) 5.4);
            _store.OpenStore(yossiLogInResult.Value, store, product);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }

        [Test]
        public void TestSuccess()
        {
            string token = _auth.Connect();
            _cart.AddItemToCart(token, "Tara milk", store, 5);
            Result result = _cart.PurchaseCart(token, new PaymentInfo());
            Assert.True(result.IsSuccess, result.Error);
            _auth.Disconnect(token);
        }
        
        [TestCase(10)]
        [TestCase(15)]
        [TestCase(-1)]
        [Test]
        public void TestFailure(int amount)
        {
            string token = _auth.Connect();
            _cart.AddItemToCart(token, "Tara milk", store, amount);
            Result result = _cart.PurchaseCart(token, new PaymentInfo());
            Assert.True(result.IsFailure, "amount invalid, process was suppose to fail amount was: " + amount);
            _auth.Disconnect(token);
        }
    }
}