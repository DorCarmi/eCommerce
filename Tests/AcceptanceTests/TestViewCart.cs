using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using eCommerce.Auth;
using eCommerce.Business;
using eCommerce.Business.Repositories;
using eCommerce.Common;
using eCommerce.Service;
using NUnit.Framework;

namespace Tests.AcceptanceTests
{
    /// <summary>
    /// <UC>
    /// View shopping cart
    /// </UC>
    /// <Req>
    /// 2.8
    /// </Req>
    /// </summary>
    [TestFixture]
    [Order(18)]
    public class TestViewCart
    {
        private IAuthService _auth;
        private IStoreService _store;
        private ICartService _cart;
        private string store = "Yossi's Store";


        [SetUpAttribute]
        public async Task SetUp()
        {
            StoreRepository SR = new StoreRepository();
            InMemoryRegisteredUserRepo RP = new InMemoryRegisteredUserRepo();
            UserAuth UA = UserAuth.CreateInstanceForTests(RP);
            IRepository<User> UR = new InMemoryRegisteredUsersRepository();

            _auth = AuthService.CreateUserServiceForTests(UA, UR, SR);
            _store = StoreService.CreateUserServiceForTests(UA, UR, SR);
            _cart = CartService.CreateUserServiceForTests(UA, UR, SR);
            MemberInfo yossi = new MemberInfo("Yossi11", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("singerMermaid", "shiran@gmail.com", "Shiran Moris",
                DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            MemberInfo lior = new MemberInfo("Liorwork", "lior@gmail.com", "Lior Lee",
                DateTime.ParseExact("05/07/1996", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Carl Neter 14");
            string token = _auth.Connect();
            await _auth.Register(token, yossi, "qwerty123");
            await _auth.Register(token, shiran, "130452abc");
            await _auth.Register(token, lior, "987654321");
            Result<string> yossiLogInResult = await _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
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
        public void TestViewCartEmpty()
        {
            string token = _auth.Connect();
            Result<SCart> result = _cart.GetCart(token);
            Assert.True(result.IsSuccess && result.Value.Baskets.Count == 0);
            _auth.Disconnect(token);
        }
        [Test]
        public void TestViewCartNonEmpty()
        {
            string token = _auth.Connect();
            _cart.AddItemToCart(token, "Tara milk", store, 3);
            Result<SCart> result = _cart.GetCart(token);
            Assert.True(result.IsSuccess && result.Value.Baskets.Count != 0);
            _auth.Disconnect(token);
        }
    }
}