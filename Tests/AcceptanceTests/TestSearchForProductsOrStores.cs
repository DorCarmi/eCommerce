﻿using System;
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
        private IStoreService _store;
        private string store = "Barovia";
        
        
        [SetUpAttribute]
        public void SetUp()
        {
            StoreRepository SR = new StoreRepository();
            TRegisteredUserRepo RP = new TRegisteredUserRepo();
            UserAuth UA = UserAuth.CreateInstanceForTests(RP);
            IRepository<IUser> UR = new RegisteredUsersRepository();

            _auth = AuthService.CreateUserServiceForTests(UA, UR, SR);
            _store = StoreService.CreateUserServiceForTests(UA, UR, SR);
            MemberInfo yossi = new MemberInfo("Strahd", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            string token = _auth.Connect();
            _auth.Register(token, yossi, "qwerty123");
            Result<string> yossiLogInResult = _auth.Login(token, "Strahd", "qwerty123", ServiceUserRole.Member);
            IItem product = new SItem("Tara milk", store, 10, "dairy",
                new List<string>{"dairy", "milk", "Tara"}, (double)5.4);
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
        
        [TestCase("Barovia")]
        public void TestExistsStore(string query)
        {
            string token = _auth.Connect();
            Result<IEnumerable<string>> result = _store.SearchForStore(token, query);
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
            Result<IEnumerable<IItem>> result = _store.SearchForItem(token, query);
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
            Result<IEnumerable<string>> result = _store.SearchForStore(token, query);
            Assert.False(result.IsSuccess && result.Value.GetEnumerator().MoveNext(), "Query \"" + query + "\" returned a non-empty list!");
            _auth.Disconnect(token);
        }
    }
}