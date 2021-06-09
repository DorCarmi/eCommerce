﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using eCommerce.Auth;
using System.Threading.Tasks;
using eCommerce.Business;
using eCommerce.Business.Repositories;
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
    [Order(17)]
    public class TestSearchForProductsOrStores
    {
        private IAuthService _auth;
        private INStoreService _inStore;
        private string store = "Barovia";
        
        
        [SetUpAttribute]
        public async Task SetUp()
        {
            InMemoryRegisteredUserRepo RP = new InMemoryRegisteredUserRepo();
            UserAuth UA = UserAuth.CreateInstanceForTests(RP, "ThisKeyIsForTests");
            InMemoryStoreRepo SR = new InMemoryStoreRepo();
            IRepository<User> UR = new InMemoryRegisteredUsersRepository();
            IMarketFacade marketFacade = MarketFacade.CreateInstanceForTests(UA,UR, SR);

            _auth = AuthService.CreateUserServiceForTests(marketFacade);
            _inStore = InStoreService.CreateUserServiceForTests(marketFacade);
            MemberInfo yossi = new MemberInfo("Strahd", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            string token = _auth.Connect();
            await _auth.Register(token, yossi, "qwerty123");
            Result<string> yossiLogInResult = await _auth.Login(token, "Strahd", "qwerty123", ServiceUserRole.Member);
            IItem product = new SItem("Tara milk", store, 10, "dairy",
                new List<string>{"dairy", "milk", "Tara"}, (double)5.4);
            _inStore.OpenStore(yossiLogInResult.Value, store);
            _inStore.AddNewItemToStore(yossiLogInResult.Value, product);
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
        [TestCase("Tara")]
        [TestCase("milk")]
        [Test]
        public void TestExistsProduct(string query)
        {
            string token = _auth.Connect();
            Result<IEnumerable<IItem>> result = _inStore.SearchForItem(token, query);
            Assert.True(result.IsSuccess && result.Value.GetEnumerator().MoveNext(), "Query \"" + query + "\" returned an empty list!");
            _auth.Disconnect(token);
        }
        
        [TestCase("Barovia")]
        public void TestExistsStore(string query)
        {
            string token = _auth.Connect();
            Result<IEnumerable<string>> result = _inStore.SearchForStore(token, query);
            Assert.True(result.IsSuccess && result.Value.GetEnumerator().MoveNext(), "Query \"" + query + "\" returned an empty list!");
            _auth.Disconnect(token);
        }
        
        [TestCase("Red dragon Crush")]
        [TestCase("Mermaid")]
        [Order(1)]
        [Test]
        public void TestNotExistsProduct(string query)
        {
            string token = _auth.Connect();
            Result<IEnumerable<IItem>> result = _inStore.SearchForItem(token, query);
            Assert.False(result.IsSuccess && result.Value.GetEnumerator().MoveNext(), "Query \"" + query + "\" returned a non-empty list!");
            _auth.Disconnect(token);
        }
        
        [TestCase("Red dragon Crush")]
        [TestCase("Mermaid")]
        //TODO: Check
        [Order(0)]
        [Test]
        public void TestNotExistsStore(string query)
        {
            string token = _auth.Connect();
            Result<IEnumerable<string>> result = _inStore.SearchForStore(token, query);
            Assert.False(result.IsSuccess && result.Value.GetEnumerator().MoveNext(), "Query \"" + query + "\" returned a non-empty list!");
            _auth.Disconnect(token);
        }
    }
}