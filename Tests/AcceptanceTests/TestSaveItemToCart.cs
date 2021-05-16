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
    /// Save items in a shopping cart
    /// </UC>
    /// <Req>
    /// 2.7
    /// </Req>
    /// </summary>
    [TestFixture]
    [Order(4)]
    public class TestSaveItemToCart
    {
        private IAuthService _auth;
        private IStoreService _store;
        private IUserService _user;
        private ICartService _cart;
        private string store = "Yossi's Store";
        
        
        [SetUp]
        public void SetUp()
        {
            _auth = new AuthService();
            _store = new StoreService();
            _user = new UserService();
            _cart = new CartService();
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
            IItem product = new SItem("Tara milk", store, 10, "dairy",
                new List<string>{"dairy", "milk", "Tara"}, (double)5.4);
            
            _store.OpenStore(yossiLogInResult.Value, store);
            _store.AddNewItemToStore(yossiLogInResult.Value, product);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        //TODO:Check
        [TestCase("Tara milk", "Yossi's store", 3)]
        [TestCase("Tara milk", "Yossi's store", 5)]
        [Test]
        public void TestAddItemToCartSuccess(string itemId, string storeName, int amount)
        { 
            string token = _auth.Connect();
            Result<string> login = _auth.Login(token, "singerMermaid", "130452abc", ServiceUserRole.Member);
            Assert.True(login.IsSuccess, login.Error);
            Result result = _cart.AddItemToCart(login.Value, itemId, storeName, amount);
            Assert.True(result.IsSuccess, "failed to add  " + " from " + storeName + " to cart: " + result.Error);
            token = _auth.Logout(login.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("Tnuva cream cheese", "Yossi's store", 3)]
        [TestCase("Tara milk", "Yossi's store", -3)]
        [TestCase("Tara milk", "Yossi's store", 12)]
        [Test] 
        public void TestAddItemToCartFailure(string itemId, string storeName, int amount)
        { 
            string token = _auth.Connect();
            Result<string> login = _auth.Login(token, "singerMermaid", "130452abc", ServiceUserRole.Member);
            Result result = _cart.AddItemToCart(login.Value, itemId, storeName, amount);
            Assert.True(result.IsFailure, "action was suppose to fail!");
            token = _auth.Logout(login.Value).Value;
            _auth.Disconnect(token);
        }
    }
}