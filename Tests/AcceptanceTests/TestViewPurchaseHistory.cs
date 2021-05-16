using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using eCommerce.Business;
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
    [Order(0)]
    public class TestViewPurchaseHistory
    {
        private IAuthService _auth;
        private IStoreService _store;
        private IUserService _user;
        private ICartService _cart;
        private string storeName = "Yossi's Store";

        [SetUp]
        public void SetUp()
        {
            _auth = new AuthService();
            _store = new StoreService();
            _user = new UserService();
            _cart = new CartService();
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
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }

        [Test] 
        public void TestSuccessEmpty()
        {
            string token = _auth.Connect();
            Result<string> yossiLogInResult = _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result<SPurchaseHistory> result = _user.GetPurchaseHistory(yossiLogInResult.Value);
            Assert.True(result.IsSuccess && result.Value.Records.Count == 0);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        [Test] 
        public void TestSuccessNonEmpty()
        {
            string token = _auth.Connect();
            Result<string> yossiLogInResult = _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            _cart.AddItemToCart(yossiLogInResult.Value, "Tara milk", storeName, 5);
            _cart.PurchaseCart(yossiLogInResult.Value, new PaymentInfo("Yossi11","123456789","1234567890123456","12/34","123","address"));
            Result<SPurchaseHistory> result = _user.GetPurchaseHistory(yossiLogInResult.Value);
            Assert.True(result.IsSuccess && result.Value.Records.Count != 0);
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        } 
    }
}