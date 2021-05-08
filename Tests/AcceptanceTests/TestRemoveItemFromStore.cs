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
    /// Remove product from store
    /// </UC>
    /// <Req>
    /// 4.1
    /// </Req>
    /// </summary>
    [TestFixture]
    public class TestRemoveItemFromStore
    {
        private IAuthService _auth;
        private IStoreService _store;
        private string storeName = "Yossi's Store";

        [SetUp]
        public void SetUp()
        {
            _auth = new AuthService();
            _store = new StoreService();
            MemberInfo yossi = new MemberInfo("Yossi11", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("singerMermaid", "shiran@gmail.com", "Shiran Moris",
                DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            string token = _auth.Connect();
            _auth.Register(token, yossi, "qwerty123");
            _auth.Register(token, shiran, "130452abc");
            Result<string> yossiLogInResult = _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            IItem product = new SItem("Tara milk", storeName, 10, "dairy",
                new ReadOnlyCollection<string>(new List<string>{"dairy", "milk", "Tara"}), (double)5.4);
            _store.OpenStore(yossiLogInResult.Value, storeName);
            _store.AddNewItemToStore(yossiLogInResult.Value, product);
            _store.AddNewItemToStore(yossiLogInResult.Value, new SItem("iPhone X", storeName, 35, "smartphones", 
                new ReadOnlyCollection<string>(new List<string>{"smartphone", "iPhone", "Apple", "Iphone X"}), (double) 5000.99));
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("Tara milk")]
        [TestCase("iPhone X")]
        [Test]
        public void TestSuccess(string productName)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result removeItemResult = _store.RemoveItemFromStore(yossiLogin.Value, storeName, productName);
            Assert.True(removeItemResult.IsSuccess, "failed to remove item " + productName + ": " + removeItemResult.Error);
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token); 
        }
        
        [TestCase("Yossi's Store", "Gans 356 air", "Yossi11", "qwerty123")]
        [TestCase("dancing doors", "Tara milk", "Yossi11", "qwerty123")] // non existing store
        [TestCase("Yossi's Store", "Gans 356 air", "singerMermaid", "130452abc")]
        [Test]      
        public void TestFailureInvalid(string store, string productName, string member, string password)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = _auth.Login(token, member, password, ServiceUserRole.Member);
            Result removeItemResult = _store.RemoveItemFromStore(yossiLogin.Value, store, productName);
            Assert.True(removeItemResult.IsFailure);
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token); 
        }

        [Test]
        public void TestRemoveLastProduct()
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            _store.RemoveItemFromStore(yossiLogin.Value, storeName, "Tara milk");
            Result removeItemResult = _store.RemoveItemFromStore(yossiLogin.Value, storeName, "iPhone X");
            Assert.True(removeItemResult.IsFailure, "can't remove all products from a store");
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token); 
        }
        
        [TestCase("Tara milk")]
        [TestCase("iPhone X")]
        [Test]
        public void TestFailureLogic(string productName)
        {
            string token = _auth.Connect();
            Result removeItemResult = _store.RemoveItemFromStore(token, storeName, productName);
            Assert.True(removeItemResult.IsFailure, "Suppose to fail, user not logged in");
            _auth.Disconnect(token); 
        }
    }
}