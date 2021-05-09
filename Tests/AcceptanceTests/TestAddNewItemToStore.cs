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
    /// Add new product to store
    /// </UC>
    /// <Req>
    /// 4.1
    /// </Req>
    /// </summary>
    
    [TestFixture]
    public class TestAddNewItemToStore
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
            _store.OpenStore(yossiLogInResult.Value, storeName);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("iPhone X", 35, "smartphones",
            new string[] {"smartphone", "iPhone", "Apple", "Iphone X"}, (double) 5000.99)]
        [TestCase("Gans 356 air Rubik's cube", 178, "games",
            new string[] {"games", "Rubik's cube", "Gans","356 air"}, (double) 114.75)]
        [Test]
        public void TestSuccess(string name, int amount, string category, string[] tags,
            double price)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result addItemResult = _store.AddNewItemToStore(yossiLogin.Value,
                new SItem(name, storeName, amount, category, new List<string>(tags), price));
            Assert.True(addItemResult.IsSuccess, "failed to add item: " + name + "|error: " + addItemResult.Error);
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("iPhone X", "Yossi's Store", -23, "smartphones",
            new string[] {"smartphone", "iPhone", "Apple", "Iphone X"}, (double) 5000.99)]
        [TestCase("Gans 356 air Rubik's cube", "Yossi's Store", 178, "games",
            new string[] {"games", "Rubik's cube", "Gans","356 air"}, (double) -75.9)]
        [TestCase("Cube Alarm", "the dancing pirate", 5986, "electronics",
            new string[] {"alarm", "electronics", "cube","decorations"}, (double) 65.5)]
        [Test]      
        public void TestFailureInput(string name, string store, int amount, string category, string[] tags,
            double price)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result addItemResult = _store.AddNewItemToStore(yossiLogin.Value,
                new SItem(name, storeName, amount, category, new List<string>(tags), price));
            Assert.True(addItemResult.IsFailure, "item addition was suppose to fail for " + name);
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("iPhone X", 35, "smartphones",
            new string[] {"smartphone", "iPhone", "Apple", "Iphone X"}, 000.99, "dancing dragon")]
        [TestCase("Gans 356 air Rubik's cube", 178, "games",
            new string[] {"games", "Rubik's cube", "Gans","356 air"}, 114.75, "Yossi's store")]
        [Test]      
        public void TestFailureLogic(string name, int amount, string category, string[] tags,
            double price, string store)
        {
            string token = _auth.Connect();
            Result<string> login = _auth.Login(token, "singerMermaid", "130452abc", ServiceUserRole.Member);
            Result addItemResult = _store.AddNewItemToStore(login.Value,
                new SItem(name, store, amount, category, new List<string>(tags), price));
            Assert.True(addItemResult.IsFailure, "item addition was suppose to fail for " + name + ", since the user does not own the store.");
            token = _auth.Logout(login.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("Tara milk", "Yossi's Store", 10, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)5.8)]
        [Test]      
        public void TestFailureInputDoubleAddition(string name, string store, int amount, string category, string[] tags,
            double price)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            _store.AddNewItemToStore(yossiLogin.Value, new SItem(name, storeName, amount, category, new List<string>(tags), price));
            Result addItemResult = _store.AddNewItemToStore(yossiLogin.Value,
                new SItem(name, storeName, amount, category, new List<string>(tags), price));
            Assert.True(addItemResult.IsFailure, "item addition was suppose to fail for " + name);
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("iPhone X", 35, "smartphones",
            new string[] {"smartphone", "iPhone", "Apple", "Iphone X"}, (double) 5000.99)]
        [TestCase("Gans 356 air Rubik's cube", 178, "games",
            new string[] {"games", "Rubik's cube", "Gans","356 air"}, (double) 114.75)]
        [Test]      
        public void TestFailureLogicNoLogin(string name, int amount, string category, string[] tags,
            double price)
        {
            string token = _auth.Connect();
            Result addItemResult = _store.AddNewItemToStore(token,
                new SItem(name, storeName, amount, category, new List<string>(tags), price));
            Assert.True(addItemResult.IsFailure, "item addition was suppose to fail for " + name + ", since the user is not logged in.");
            _auth.Disconnect(token);
        }
        
    }
}