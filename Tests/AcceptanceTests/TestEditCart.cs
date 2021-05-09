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
    /// Edit shopping cart
    /// </UC>
    /// <Req>
    /// 2.8
    /// </Req>
    /// </summary>
    [TestFixture]
    public class TestEditCart
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
            MemberInfo shiran = new MemberInfo("singerMermaid", "shiran@gmail.com", "Shiran Moris",
                DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            MemberInfo lior = new MemberInfo("Liorwork", "lior@gmail.com", "Lior Lee",
                DateTime.ParseExact("05/07/1996", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Carl Neter 14");
            string token = _auth.Connect();
            _auth.Register(token, yossi, "qwerty123");
            _auth.Register(token, shiran, "130452abc");
            _auth.Register(token, lior, "987654321");
            Result<string> yossiLogInResult = _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            IItem product = new SItem("Tara milk", store, 10, "dairy",
                new List<string> {"dairy", "milk", "Tara"}, (double) 5.4);
            _store.OpenStore(yossiLogInResult.Value, store);
            _store.AddNewItemToStore(yossiLogInResult.Value, product);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("Tara milk", "Yossi's store", 9)]
        [TestCase("Tara milk", "Yossi's store", 3)]
        [TestCase("Tara milk", "Yossi's store", 0)]
        [TestCase("Tara milk", "Yossi's store", -6)]
        [Test]
        public void EditItemAmountOfCart(string itemId, string storeName, int amount)
        {
            string token = _auth.Connect();
            _cart.AddItemToCart(token, itemId, storeName, 5);
             Result result = _cart.EditItemAmountOfCart(token, itemId, storeName, amount);
            Assert.True(result.IsSuccess, "failed to edit item: " + result.Error);
            _auth.Disconnect(token);
        }
        
        [TestCase("Tnuva cream cheese", "Yossi's store", 3)]
        [TestCase("Tara milk", "dancing dragon", 0)]
        [TestCase("Tara milk", "Yossi's store", 15)]
        [Test]
        public void EditItemAmountOfCartFailure(string itemId, string storeName, int amount)
        {
            string token = _auth.Connect();
            _cart.AddItemToCart(token, itemId, storeName, 5);
            Result result = _cart.EditItemAmountOfCart(token, itemId, storeName, amount);
            Assert.True(result.IsFailure);
            _auth.Disconnect(token);
        }
    }
}