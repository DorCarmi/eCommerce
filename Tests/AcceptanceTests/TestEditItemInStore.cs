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
    /// Update product stock- add items
    /// Update product stock- subtract items
    /// Update existing product's details
    /// </UC>
    /// <Req>
    /// 4.1
    /// </Req>
    /// </summary>
    [TestFixture]
    public class TestEditItemInStore
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
        
        [TestCase("Tara milk", 15, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)5.4)] //edit amount (add)
        [TestCase("Tara milk", 5, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)5.4)] //edit amount (subtract)
        [TestCase("Tara milk", 0, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)5.4)] //edit amount (remove)
        [TestCase("Tara milk", 10, "Tara",
            new string[]{"dairy", "milk", "Tara"}, (double)5.4)] //edit category
        [TestCase("Tara milk", 10, "dairy",
            new string[]{"milk", "Tara"}, (double)5.4)] //edit keywords
        [TestCase("Tara milk", 10, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)6.2)] // edit price
        [Test]
        public void TestSuccess(string name, int amount, string category, string[] tags,
            double price)  
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result editItemResult = _store.EditItemInStore(yossiLogin.Value,
                new SItem(name, storeName, amount, category, Array.AsReadOnly(tags), price));
            Assert.True(editItemResult.IsSuccess, "failed to edit item: " + editItemResult.Error);
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("Tara milk", "Yossi's Store", -23, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)5.4)] //edit amount (invalid subtract)
        [TestCase("Tara milk", "Yossi's Store", 10, "~~123~~~Tara",
            new string[]{"dairy", "milk", "Tara"}, (double)5.4)] //edit invalid category
        [TestCase("Tara milk", "Yossi's Store", 10, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)-6.2)] // edit invalid price
        [TestCase("Gans 356 air Rubik's cube", "Yossi's Store", 178, "games",
            new string[] {"games", "Rubik's cube", "Gans","356 air"}, (double) 114.75)] // edit a non-existing item
        [TestCase("Tara milk", "prancing dragon", 10, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)5.4)] //edit fail, can't change the store (store doesn't exist)
        [Test] 
        public void TestFailureInvalid(string name, string store, int amount, string category, string[] tags,
            double price)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result editItemResult = _store.EditItemInStore(yossiLogin.Value,
                new SItem(name, store, amount, category, Array.AsReadOnly(tags), price));
            Assert.True(editItemResult.IsFailure, "was suppose to fail to edit item");
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("Tara milk", 15, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)5.4)]
        [Test] 
        public void TestFailureNotOwner(string name, int amount, string category, string[] tags,
            double price)
        {
            string token = _auth.Connect();
            Result<string> login = _auth.Login(token, "singerMermaid", "130452abc", ServiceUserRole.Member);
            Result editItemResult = _store.EditItemInStore(login.Value,
                new SItem(name, storeName, amount, category, Array.AsReadOnly(tags), price));
            Assert.True(editItemResult.IsFailure, "was suppose to fail to edit item, user doesn't own the store");
            token = _auth.Logout(login.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("Tara milk", 10, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)6.2)]
        [Test]
        public void TestFailureNotLoggedIn(string name, int amount, string category, string[] tags,
            double price)  
        {
            string token = _auth.Connect();
            Result editItemResult = _store.EditItemInStore(token,
                new SItem(name, storeName, amount, category, Array.AsReadOnly(tags), price));
            Assert.True(editItemResult.IsFailure, "was suppose to fail. user is not logged in");
            _auth.Disconnect(token);
        }
    }
}