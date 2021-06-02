using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using eCommerce.Auth;
using eCommerce.Adapters;
using System.Threading.Tasks;
using eCommerce.Business;
using eCommerce.Common;
using eCommerce.Service;
using NUnit.Framework;
using Tests.AuthTests;
using Tests.Service;

namespace Tests.AcceptanceTests
{
    /// <summary>
    /// <UC>
    /// Purchase the whole cart
    /// </UC>
    /// <Req>
    /// 2.9
    /// </Req>
    /// </summary>
    [TestFixture]
    public class TestBuyCart
    {
        private IAuthService _auth;
        private IStoreService _store;
        private ICartService _cart;
        private IUserService _user;
        private string store_name = "Borca";


        [SetUp]
        public async Task SetUp()
        {
            PaymentProxy.AssignPaymentService(new mokPaymentService(true,true,true));
            SupplyProxy.AssignSupplyService(new mokSupplyService(true,true));
                _auth = new AuthService();
            _store = new StoreService();
            _cart = new CartService();
            _user = new UserService();
            StoreRepository SR = new StoreRepository();
            InMemoryRegisteredUserRepo RP = new InMemoryRegisteredUserRepo();
            UserAuth UA = UserAuth.CreateInstanceForTests(RP);
            IRepository<IUser> UR = new RegisteredUsersRepository();

            _auth = AuthService.CreateUserServiceForTests(UA, UR, SR);
            _store = StoreService.CreateUserServiceForTests(UA, UR, SR);
            _cart = CartService.CreateUserServiceForTests(UA, UR, SR);
            MemberInfo Ivan = new MemberInfo("Ivan11", "Ivan@gmail.com", "Ivan Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            string token = _auth.Connect();
            await _auth.Register(token, Ivan, "qwerty123");
            Result<string> IvanLogInResult = await _auth.Login(token, "Ivan11", "qwerty123", ServiceUserRole.Member);
            IItem product = new SItem("Tara milk", store_name, 10, "dairy",
                new List<string> {"dairy", "milk", "Tara"}, (double) 5.4);
            IItem product2 = new SItem("Chocolate milk", store_name, 200, "Sweet",
                new List<string> {"dairy", "milk", "sweet"}, (double) 3.5);
            _store.OpenStore(IvanLogInResult.Value, store_name);
            _store.AddNewItemToStore(IvanLogInResult.Value, product);
            _store.AddNewItemToStore(IvanLogInResult.Value, product2);
            token = _auth.Logout(IvanLogInResult.Value).Value;
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
        [Order(0)]
        public void TestBuyCartSuccess()
        {
            string token = _auth.Connect();
            _cart.AddItemToCart(token, "Tara milk", store_name, 5);
            Result result = _cart.PurchaseCart(token, new PaymentInfo("Ivan11","123456789","1234567890123456","12/34","123","address"));
            Assert.True(result.IsSuccess, result.Error);
            _auth.Disconnect(token);
        }
        
        [TestCase(10)]
        [TestCase(15)]
        [TestCase(-1)]
        [Order(1)]
        [Test]
        public void TestBuyCartFailure(int amount)
        {
            string token = _auth.Connect();
            _cart.AddItemToCart(token, "Tara milk", store_name, amount);
            Result result = _cart.PurchaseCart(token, new PaymentInfo("Ivan11","123456789","1234567890123456","12/34","123","address"));
            Assert.True(result.IsFailure, "amount invalid, process was suppose to fail amount was: " + amount);
            _auth.Disconnect(token);
        }
        
        //missing item
        //payment fail
        //supply fail
        [TestCase(10)]
        [Order(2)]
        public async Task TestBuyCartPaymentInfoFail(int amount)
        {
            string token = _auth.Connect();
            Result<string> IvanLogInResult = await _auth.Login(token, "Ivan11", "qwerty123", ServiceUserRole.Member);
            Assert.True(IvanLogInResult.IsSuccess,IvanLogInResult.Error);
            Result<SPurchaseHistory> historyResult = _store.GetPurchaseHistoryOfStore(IvanLogInResult.Value, store_name);
            int countStoreHistory = historyResult.Value.Records.Count;
            int userHistory = _user.GetPurchaseHistory(IvanLogInResult.Value).Value.Records.Count;
            int countRealPaymentHits = PaymentProxy.REAL_HITS;
            int countRealRefund = PaymentProxy.REAL_REFUNDS;
            int countRealSupplyHits = SupplyProxy.REAL_HITS;
            PaymentProxy.AssignPaymentService(new mokPaymentService(true,false,true));
            
            var resAddCart=_cart.AddItemToCart(IvanLogInResult.Value, "Chocolate milk", store_name, amount);
            Assert.True(resAddCart.IsSuccess,resAddCart.Error);
            Result purchaseResult = _cart.PurchaseCart(IvanLogInResult.Value,
                new PaymentInfo("Ivan11", "123456789", "1234567890123456", "12/34", "123", "address"));
            
            Assert.False(purchaseResult.IsSuccess);
            Assert.True(purchaseResult.Error.Contains("<PaymentInfo>"),purchaseResult.Error);
            Assert.AreEqual(countStoreHistory,historyResult.Value.Records.Count);
            Assert.AreEqual(userHistory,_user.GetPurchaseHistory(IvanLogInResult.Value).Value.Records.Count);
            Assert.AreEqual(countRealPaymentHits,PaymentProxy.REAL_HITS);
            Assert.AreEqual(countRealRefund,PaymentProxy.REAL_REFUNDS);
            Assert.AreEqual(countRealSupplyHits,SupplyProxy.REAL_HITS);
            _auth.Logout(IvanLogInResult.Value);
        }
        
        //missing item
        //payment fail
        //supply fail
        [TestCase(10)]
        [Order(2)]
        public async Task TestBuyCartPaymentProcessFail(int amount)
        {
            string token = _auth.Connect();
            Result<string> IvanLogInResult = await _auth.Login(token, "Ivan11", "qwerty123", ServiceUserRole.Member);
            Assert.True(IvanLogInResult.IsSuccess,IvanLogInResult.Error);
            Result<SPurchaseHistory> historyResult = _store.GetPurchaseHistoryOfStore(IvanLogInResult.Value, store_name);
            int countStoreHistory = historyResult.Value.Records.Count;
            int userHistory = _user.GetPurchaseHistory(IvanLogInResult.Value).Value.Records.Count;
            int countRealPaymentHits = PaymentProxy.REAL_HITS;
            int countRealRefund = PaymentProxy.REAL_REFUNDS;
            int countRealSupplyHits = SupplyProxy.REAL_HITS;
            PaymentProxy.AssignPaymentService(new mokPaymentService(false,true,true));
            
            var resAddCart=_cart.AddItemToCart(IvanLogInResult.Value, "Chocolate milk", store_name, amount);
            Assert.True(resAddCart.IsSuccess,resAddCart.Error);
            Result purchaseResult = _cart.PurchaseCart(IvanLogInResult.Value,
                new PaymentInfo("Ivan11", "123456789", "1234567890123456", "12/34", "123", "address"));
            
            Assert.False(purchaseResult.IsSuccess);
            Assert.False(purchaseResult.Error.Contains("<PaymentInfo>"),purchaseResult.Error);
            Assert.True(purchaseResult.Error.Contains("<Payment>"),purchaseResult.Error);
            Assert.AreEqual(countStoreHistory,historyResult.Value.Records.Count);
            Assert.AreEqual(userHistory,_user.GetPurchaseHistory(IvanLogInResult.Value).Value.Records.Count);
            Assert.AreEqual(countRealPaymentHits,PaymentProxy.REAL_HITS);
            Assert.AreEqual(countRealRefund,PaymentProxy.REAL_REFUNDS);
            Assert.AreEqual(countRealSupplyHits,SupplyProxy.REAL_HITS);
            _auth.Logout(IvanLogInResult.Value);
        }
        
        [TestCase(10)]
        [Order(3)]
        public async Task TestBuyCartSupplyInfoFail(int amount)
        {
            string token = _auth.Connect();
            Result<string> IvanLogInResult = await _auth.Login(token, "Ivan11", "qwerty123", ServiceUserRole.Member);
            Assert.True(IvanLogInResult.IsSuccess,IvanLogInResult.Error);
            Result<SPurchaseHistory> historyResult = _store.GetPurchaseHistoryOfStore(IvanLogInResult.Value, store_name);
            int countStoreHistory = historyResult.Value.Records.Count;
            int userHistory = _user.GetPurchaseHistory(IvanLogInResult.Value).Value.Records.Count;
            int countRealPaymentHits = PaymentProxy.REAL_HITS;
            int countRealRefund = PaymentProxy.REAL_REFUNDS;
            int countRealSupplyHits = SupplyProxy.REAL_HITS;
            
            PaymentProxy.AssignPaymentService(new mokPaymentService(true,true,true));
            SupplyProxy.AssignSupplyService(new mokSupplyService(false,true));
            
            var resAddCart=_cart.AddItemToCart(IvanLogInResult.Value, "Chocolate milk", store_name, amount);
            Assert.True(resAddCart.IsSuccess,resAddCart.Error);
            Result purchaseResult = _cart.PurchaseCart(IvanLogInResult.Value,
                new PaymentInfo("Ivan11", "123456789", "1234567890123456", "12/34", "123", "address"));
            
            Assert.False(purchaseResult.IsSuccess);
            Assert.True(purchaseResult.Error.Contains("<SupplyInfo>"),purchaseResult.Error);
            Assert.AreEqual(countStoreHistory,historyResult.Value.Records.Count);
            Assert.AreEqual(userHistory,_user.GetPurchaseHistory(IvanLogInResult.Value).Value.Records.Count);
            Assert.AreEqual(countRealPaymentHits+1,PaymentProxy.REAL_HITS);
            Assert.GreaterOrEqual(countRealRefund+1,PaymentProxy.REAL_REFUNDS);
            Assert.AreEqual(countRealSupplyHits,SupplyProxy.REAL_HITS);
            _auth.Logout(IvanLogInResult.Value);
        }
        
        [TestCase(10)]
        [Order(3)]
        public async Task TestBuyCartSupplyProcessFail(int amount)
        {
            string token = _auth.Connect();
            Result<string> IvanLogInResult = await _auth.Login(token, "Ivan11", "qwerty123", ServiceUserRole.Member);
            Assert.True(IvanLogInResult.IsSuccess,IvanLogInResult.Error);
            Result<SPurchaseHistory> historyResult = _store.GetPurchaseHistoryOfStore(IvanLogInResult.Value, store_name);
            int countStoreHistory = historyResult.Value.Records.Count;
            int userHistory = _user.GetPurchaseHistory(IvanLogInResult.Value).Value.Records.Count;
            int countRealPaymentHits = PaymentProxy.REAL_HITS;
            int countRealRefund = PaymentProxy.REAL_REFUNDS;
            int countRealSupplyHits = SupplyProxy.REAL_HITS;
            
            PaymentProxy.AssignPaymentService(new mokPaymentService(true,true,true));
            SupplyProxy.AssignSupplyService(new mokSupplyService(true,false));
            
            var resAddCart=_cart.AddItemToCart(IvanLogInResult.Value, "Chocolate milk", store_name, amount);
            Assert.True(resAddCart.IsSuccess,resAddCart.Error);
            Result purchaseResult = _cart.PurchaseCart(IvanLogInResult.Value,
                new PaymentInfo("Ivan11", "123456789", "1234567890123456", "12/34", "123", "address"));
            
            Assert.False(purchaseResult.IsSuccess);
            Assert.False(purchaseResult.Error.Contains("<SupplyInfo>"),purchaseResult.Error);
            Assert.True(purchaseResult.Error.Contains("<Supply>"),purchaseResult.Error);
            Assert.AreEqual(countStoreHistory,historyResult.Value.Records.Count);
            Assert.AreEqual(userHistory,_user.GetPurchaseHistory(IvanLogInResult.Value).Value.Records.Count);
            Assert.AreEqual(countRealPaymentHits+1,PaymentProxy.REAL_HITS);
            Assert.GreaterOrEqual(countRealRefund+1,PaymentProxy.REAL_REFUNDS);
            Assert.AreEqual(countRealSupplyHits,SupplyProxy.REAL_HITS);
            _auth.Logout(IvanLogInResult.Value);
        }
        
    }
}