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
    /// Gather information about store/product
    /// Search for products
    /// </UC>
    /// <Req>
    /// 2.5
    /// 2.6
    /// </Req>
    /// </summary>
    [TestFixture]
    public class TestSearchForProductsOrStores
    {
        private IAuthService _auth;
        private IStoreService _store;
        private IUserService _user;
        private string store = "Yossi's Store";
        
        
        [SetUp]
        public void SetUp()
        {
            _auth = new AuthService();
            _store = new StoreService();
            _user = new UserService();
            MemberInfo yossi = new MemberInfo("Yossi11", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            string token = _auth.Connect();
            _auth.Register(token, yossi, "qwerty123");
            Result<string> yossiLogInResult = _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            IItem product = new SItem("Tara milk", store, 10, "dairy",
                new List<string>{"dairy", "milk", "Tara"}, (double)5.4);
            _store.OpenStore(yossiLogInResult.Value, store);
            _store.AddNewItemToStore(yossiLogInResult.Value, product);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }

        [TestCase("Tara milk")]
        [TestCase("Tara")]
        [TestCase("milk")]
        [Test]
        public void TestExistsProduct(string query)
        {
            string token = _auth.Connect();
            Result<IEnumerable<IItem>> result = _store.SearchForItem(token, query);
            Assert.True(result.IsSuccess && result.Value.GetEnumerator().MoveNext(), "Query \"" + query + "\" returned an empty list!");
            _auth.Disconnect(token);
        }
        
        [TestCase("Yossi's Store")]
        public void TestExistsStore(string query)
        {
            string token = _auth.Connect();
            Result<IEnumerable<string>> result = _store.SearchForStore(token, query);
            Assert.True(result.IsSuccess && result.Value.GetEnumerator().MoveNext(), "Query \"" + query + "\" returned an empty list!");
            _auth.Disconnect(token);
        }
        
        [TestCase("iPhone")]
        [TestCase("Mermaid")]
        [Test]
        public void TestNotExistsProduct(string query)
        {
            string token = _auth.Connect();
            Result<IEnumerable<IItem>> result = _store.SearchForItem(token, query);
            Assert.False(result.IsSuccess && result.Value.GetEnumerator().MoveNext(), "Query \"" + query + "\" returned a non-empty list!");
            _auth.Disconnect(token);
        }
        
        [TestCase("iPhone")]
        [TestCase("Mermaid")]
        [Test]
        public void TestNotExistsStore(string query)
        {
            string token = _auth.Connect();
            Result<IEnumerable<string>> result = _store.SearchForStore(token, query);
            Assert.False(result.IsSuccess && result.Value.GetEnumerator().MoveNext(), "Query \"" + query + "\" returned a non-empty list!");
            _auth.Disconnect(token);
        }
    }
}