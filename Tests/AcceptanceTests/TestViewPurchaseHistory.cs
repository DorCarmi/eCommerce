using System;
using System.Collections.Generic;
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
    /// Review purchase history
    /// </UC>
    /// <Req>
    /// 3.7
    /// </Req>
    /// </summary>
    [TestFixture]
    [Order(19)]
    public class TestViewPurchaseHistory
    {
        private IAuthService _auth;
        private INStoreService _inStore;
        private IUserService _user;
        private ICartService _cart;
        private string storeName = "Darkon";

        [SetUpAttribute]
        public async Task SetUp()
        {
            InMemoryStoreRepo SR = new InMemoryStoreRepo();
            InMemoryRegisteredUserRepo RP = new InMemoryRegisteredUserRepo();
            UserAuth UA = UserAuth.CreateInstanceForTests(RP);
            IRepository<User> UR = new InMemoryRegisteredUsersRepository();

            _auth = AuthService.CreateUserServiceForTests(UA, UR, SR);
            _inStore = InStoreService.CreateUserServiceForTests(UA, UR, SR);
            _user = UserService.CreateUserServiceForTests(UA, UR, SR);
            _cart = CartService.CreateUserServiceForTests(UA, UR, SR);
            MemberInfo yossi = new MemberInfo("AzalinRex", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("Adrie", "shiran@gmail.com", "Shiran Moris",
                DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            string token = _auth.Connect();
            await _auth.Register(token, yossi, "qwerty123");
            await _auth.Register(token, shiran, "130452abc");
            Result<string> yossiLogInResult = await _auth.Login(token, "AzalinRex", "qwerty123", ServiceUserRole.Member);
            IItem product = new SItem("Tara milk", storeName, 10, "dairy",
                new List<string>{"dairy", "milk", "Tara"}, (double)5.4);
            _inStore.OpenStore(yossiLogInResult.Value, storeName);
            _inStore.AddNewItemToStore(yossiLogInResult.Value, product);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TearDownAttribute]
        public void Teardown()
        {
            _auth = null;
            _inStore = null;
            _user = null;
            _cart = null;
        }

        [Test] 
        public async Task TestViewPurchaseHistorySuccessEmpty()
        {
            string token = _auth.Connect();
            Result<string> yossiLogInResult = await _auth.Login(token, "AzalinRex", "qwerty123", ServiceUserRole.Member);
            Result<SPurchaseHistory> result = _user.GetPurchaseHistory(yossiLogInResult.Value);
            Assert.True(result.IsSuccess && result.Value.Records.Count == 0);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        [Test] 
        public async Task TestViewPurchaseHistorySuccessNonEmpty()
        {
            string token = _auth.Connect();
            Result<string> yossiLogInResult = await _auth.Login(token, "AzalinRex", "qwerty123", ServiceUserRole.Member);
            _cart.AddItemToCart(yossiLogInResult.Value, "Tara milk", storeName, 5);
            _cart.PurchaseCart(yossiLogInResult.Value, new PaymentInfo("AzalinRex","123456789","1234567890123456","12/34","123","address"));
            Result<SPurchaseHistory> result = _user.GetPurchaseHistory(yossiLogInResult.Value);
            Assert.True(result.IsSuccess && result.Value.Records.Count != 0);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        } 
    }
}