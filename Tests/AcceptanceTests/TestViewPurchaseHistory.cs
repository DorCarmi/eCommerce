using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
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
        private IStoreService _store;
        private IUserService _user;
        private ICartService _cart;
        private string storeName = "Darkon";

        [SetUpAttribute]
        public void SetUp()
        {
            StoreRepository SR = new StoreRepository();
            TRegisteredUserRepo RP = new TRegisteredUserRepo();
            UserAuth UA = UserAuth.CreateInstanceForTests(RP);
            IRepository<IUser> UR = new RegisteredUsersRepository();

            _auth = AuthService.CreateUserServiceForTests(UA, UR, SR);
            _store = StoreService.CreateUserServiceForTests(UA, UR, SR);
            _user = UserService.CreateUserServiceForTests(UA, UR, SR);
            _cart = CartService.CreateUserServiceForTests(UA, UR, SR);
            MemberInfo yossi = new MemberInfo("AzalinRex", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("Adrie", "shiran@gmail.com", "Shiran Moris",
                DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            string token = _auth.Connect();
            _auth.Register(token, yossi, "qwerty123");
            _auth.Register(token, shiran, "130452abc");
            Result<string> yossiLogInResult = _auth.Login(token, "AzalinRex", "qwerty123", ServiceUserRole.Member);
            IItem product = new SItem("Tara milk", storeName, 10, "dairy",
                new List<string>{"dairy", "milk", "Tara"}, (double)5.4);
            _store.OpenStore(yossiLogInResult.Value, storeName);
            _store.AddNewItemToStore(yossiLogInResult.Value, product);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TearDownAttribute]
        public void Teardown()
        {
            _auth = null;
            _store = null;
            _user = null;
            _cart = null;
        }

        [Test] 
        public void TestViewPurchaseHistorySuccessEmpty()
        {
            string token = _auth.Connect();
            Result<string> yossiLogInResult = _auth.Login(token, "AzalinRex", "qwerty123", ServiceUserRole.Member);
            Result<SPurchaseHistory> result = _user.GetPurchaseHistory(yossiLogInResult.Value);
            Assert.True(result.IsSuccess && result.Value.Records.Count == 0);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        [Test] 
        public void TestViewPurchaseHistorySuccessNonEmpty()
        {
            string token = _auth.Connect();
            Result<string> yossiLogInResult = _auth.Login(token, "AzalinRex", "qwerty123", ServiceUserRole.Member);
            _cart.AddItemToCart(yossiLogInResult.Value, "Tara milk", storeName, 5);
            _cart.PurchaseCart(yossiLogInResult.Value, new PaymentInfo("AzalinRex","123456789","1234567890123456","12/34","123","address"));
            Result<SPurchaseHistory> result = _user.GetPurchaseHistory(yossiLogInResult.Value);
            Assert.True(result.IsSuccess && result.Value.Records.Count != 0);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        } 
    }
}