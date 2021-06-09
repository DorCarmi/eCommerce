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
using Tests.AuthTests;

namespace Tests.AcceptanceTests
{
    /// <summary>
    /// <UC>
    /// Calculate total price for basket
    /// </UC>
    /// <Req>
    /// 2.9
    /// </Req>
    /// </summary>
    [TestFixture]
    [Order(6)]
    public class TestCartPrice
    {
        private IAuthService _auth;
        private INStoreService _inStore;
        private ICartService _cart;
        private string store = "Yossi's Store";


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
            _cart = CartService.CreateUserServiceForTests(marketFacade);
            MemberInfo yossi = new MemberInfo("Yossi11", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            string token = _auth.Connect();
            await _auth.Register(token, yossi, "qwerty123");
            Result<string> yossiLogInResult = await _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            IItem product = new SItem("Tara milk", store, 10, "dairy",
                new List<string> {"dairy", "milk", "Tara"}, (double) 5.4);
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
            _cart = null;
        }

        [Test]
        public void TestCartPriceEmpty()
        {
            string token = _auth.Connect();
            Result<double> result = _cart.GetPurchaseCartPrice(token);
            Assert.True(result.IsSuccess && result.Value == 0.0);
            _auth.Disconnect(token);
        }
        
        [Test]
        public void TestCartPriceNonEmpty()
        {
            string token = _auth.Connect();
            _cart.AddItemToCart(token, "Tara milk", store, 3);
            Result<double> result = _cart.GetPurchaseCartPrice(token);
            Assert.True(result.IsSuccess && result.Value != 0.0);
            _auth.Disconnect(token);
        }
    }
}