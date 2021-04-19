using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using eCommerce.Auth;
using eCommerce.Business;
using eCommerce.Business.Service;
using eCommerce.Common;
using NUnit.Framework;
using Tests.AuthTests;
using Tests.Business;

namespace Tests
{
    [TestFixture]
    public class AcceptanceTests
    {
        private IMarketFacade _market; 
        private enum categories
        {
            system,
            registeredUsers,
            stores,
            products,
            transactions
        }
        
        [SetUp]
        public void SetUp()
        {
            _market = MarketFacade.CreateInstanceForTests(UserAuth.CreateInstanceForTests(new TRegisteredUserRepo()), new RegisteredUsersRepository(), new StoreRepository());
            MemberInfo yossi = new MemberInfo("Yossi11","yossi@gmail.com", "Yossi Park", DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("singerMermaid","shiran@gmail.com", "Shiran Moris", DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            MemberInfo lior = new MemberInfo("Liorwork","lior@gmail.com", "Lior Lee", DateTime.ParseExact("05/07/1996", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Carl Neter 14");
            string token = _market.Connect();
            _market.Register(token, yossi, "qwerty123");
            _market.Register(token, shiran, "130452abc");
            _market.Register(token, lior, "987654321");
            Result<string> yossiLogInResult = _market.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            string storeName = "Yossi's Store";
            IItem product = new ItemDto("Tara milk", storeName, 10, "dairy",
                new ReadOnlyCollection<string>(new List<string>{"dairy", "milk", "Tara"}), (double)5.4);
            _market.OpenStore(yossiLogInResult.Value, storeName, product);
            token = _market.Logout(yossiLogInResult.Value).Value;
            _market.Disconnect(token);
        }
        
        
        /*
         * UC - Register to system
         * Req - 2.3
         */
        [TestCase("Tamir123","tamir@gmail.com", "Tamir", "23/03/1961", "gold st. 14", "tdddev123")]
        [TestCase("Nathan43","nat4343@gmail.com", "Nathan dor", "17/07/1997", "main st. 57", "NathanFTW765")]
        [Test]
        public void TestRegisterToSystemSuccess(string username, string email, string name, string birthday, string address, string password)
        {
            MemberInfo memberInfo = new MemberInfo(username, email, name, DateTime.ParseExact(birthday, "dd/MM/yyyy", CultureInfo.InvariantCulture), address);
            string token = _market.Connect();
            Result result = _market.Register(token, memberInfo, password);
            Assert.True(result.IsSuccess, result.Error);
            _market.Disconnect(token);
        }
        /*
         * UC - Register to system
         * Req - 2.3
         */
        [TestCase("Yossi11","yossi@gmail.com", "Yossi Park", "19/04/2005", "hazait 14", "tdddev123")]
        [TestCase("~~Nathan43~~","nat4343@gmail.com", "Nathan dor", "17/07/1997", "main st. 57", "NathanFTW765")]
        [Test]
        public void TestRegisterToSystemFailure(string username, string email, string name, string birthday, string address, string password)
        {
            MemberInfo memberInfo = new MemberInfo(username, email, name, DateTime.ParseExact(birthday, "dd/MM/yyyy", CultureInfo.InvariantCulture), address);
            string token = _market.Connect();
            Result result = _market.Register(token, memberInfo, password);
            Assert.True(result.IsFailure, username + " was suppose to fail!");
            _market.Disconnect(token);
        }

        /*
         * UC - Login
         * Req - 2.4
         */
        [TestCase("Yossi11", "qwerty123")]
        [TestCase("singerMermaid", "130452abc")]
        [Test]
        public void TestLoginSuccess(string username, string password)
        {
            string token = _market.Connect();
            Result<string> result = _market.Login(token, username, password, ServiceUserRole.Member);
            Assert.True(result.IsSuccess, result.Error);
            _market.Disconnect(token);
        }
        /*
         * UC - Login
         * Req - 2.4
         */
        [TestCase("Yossi11", "qwerty")]
        [TestCase("_singerMermaid", "130452abc")]
        [TestCase("Tamir123", "130452abc")]
        [Test]
        public void TestLoginFailure(string username, string password)
        {
            string token = _market.Connect();
            Result<string> result = _market.Login(token, username, password, ServiceUserRole.Member);
            Assert.True(result.IsFailure, "username: " + username + " | password: " + password + "| was suppose to fail!");
            _market.Disconnect(token);
        }
        /*
         * UC - Logout
         * Req - 3.1
         */
        [Test]
        public void TestLogoutSuccess()
        {
            string token = _market.Connect();
            Result<string> result = _market.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            result = _market.Logout(result.Value);
            Assert.True(result.IsSuccess, "logout failed");
            token = result.Value;
            result = _market.Login(token, "singerMermaid", "130452abc", ServiceUserRole.Member);
            result = _market.Logout(result.Value);
            Assert.True(result.IsSuccess, "logout failed");
            _market.Disconnect(result.Value);
        }
        /*
         * UC - Logout
         * Req - 3.1
         */
        [Test]
        public void TestLogoutFailure()
        {
            string token = _market.Connect();
            Result<string> result = _market.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result<string> falseResult = _market.Logout(token);
            Assert.True(falseResult.IsFailure, "logout failed");
            _market.Disconnect(result.Value);
        }
        /*
         * UC - Add new product to store
         * Req - 4.1
         */
        [TestCase("iPhone X", "Yossi's Store", 35, "smartphones",
            new string[] {"smartphone", "iPhone", "Apple", "Iphone X"}, (double) 5000.99)]
        [TestCase("Gans 356 air Rubik's cube", "Yossi's Store", 178, "games",
            new string[] {"games", "Rubik's cube", "Gans","356 air"}, (double) 114.75)]
        [Test]
        public void TestAddNewItemToStoreSuccess(string name, string storeName, int amount, string category, string[] tags,
            double price)
        {
            string token = _market.Connect();
            Result<string> yossiLogin = _market.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result addItemResult = _market.AddNewItemToStore(yossiLogin.Value,
                new ItemDto(name, storeName, amount, category, Array.AsReadOnly(tags), price));
            Assert.True(addItemResult.IsSuccess, "failed to add items: " + addItemResult.Error);
            token = _market.Logout(yossiLogin.Value).Value;
            _market.Disconnect(token);
        }
        /*
         * UC - Add new product to store
         * Req - 4.1
         */
        [TestCase("iPhone X", "Yossi's Store", -23, "smartphones",
            new string[] {"smartphone", "iPhone", "Apple", "Iphone X"}, (double) 5000.99)]
        [TestCase("Gans 356 air Rubik's cube", "Yossi's Store", 178, "games",
            new string[] {"games", "Rubik's cube", "Gans","356 air"}, (double) -75.9)]
        [TestCase("Cube Alarm", "the dancing pirate", 5986, "electronics",
            new string[] {"alarm", "electronics", "cube","decorations"}, (double) 65.5)]
        [TestCase("Tara milk", "Yossi's Store", 10, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)5.8)]
        [Test]      //TODO add a test case for a store that exists but not owned by the current user
        public void TestAddNewItemToStoreFailureInput(string name, string storeName, int amount, string category, string[] tags,
            double price)
        {
            string token = _market.Connect();
            Result<string> yossiLogin = _market.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result addItemResult = _market.AddNewItemToStore(yossiLogin.Value,
                new ItemDto(name, storeName, amount, category, Array.AsReadOnly(tags), price));
            Assert.True(addItemResult.IsFailure, "item addition was suppose to fail");
            token = _market.Logout(yossiLogin.Value).Value;
            _market.Disconnect(token);
        }
        /*
         * UC - Add new product to store
         * Req - 4.1
         */
        [TestCase("iPhone X", "Yossi's Store", 35, "smartphones",
            new string[] {"smartphone", "iPhone", "Apple", "Iphone X"}, (double) 5000.99)]
        [TestCase("Gans 356 air Rubik's cube", "Yossi's Store", 178, "games",
            new string[] {"games", "Rubik's cube", "Gans","356 air"}, (double) 114.75)]
        [Test]
        public void TestAddNewItemToStoreFailureAccess(string name, string storeName, int amount, string category, string[] tags,
            double price)
        {
            string token = _market.Connect();
            Result addItemResult = _market.AddNewItemToStore(token,
                new ItemDto(name, storeName, amount, category, Array.AsReadOnly(tags), price));
            Assert.True(addItemResult.IsFailure, "Item Addition was suppose to fail");
            token = _market.Logout(token).Value;
            _market.Disconnect(token);
        }
        /*
         * UC - Update product stock- add items
         * UC - Update product stock- subtract items
         * UC - Update existing product's details
         * Req - 4.1
         */
        [TestCase("Tara milk", "Yossi's Store", 15, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)5.4)] //edit amount (add)
        [TestCase("Tara milk", "Yossi's Store", 5, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)5.4)] //edit amount (subtract)
        [TestCase("Tara milk", "Yossi's Store", 0, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)5.4)] //edit amount (remove)
        [TestCase("Tara milk", "Yossi's Store", 10, "Tara",
            new string[]{"dairy", "milk", "Tara"}, (double)5.4)] //edit category
        [TestCase("Tara milk", "Yossi's Store", 10, "dairy",
            new string[]{"milk", "Tara"}, (double)5.4)] //edit keywords
        [TestCase("Tara milk", "Yossi's Store", 10, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)6.2)] // edit price
        [Test]
        public void TestEditItemInStoreSuccess(string name, string storeName, int amount, string category, string[] tags,
            double price)  
        {
            string token = _market.Connect();
            Result<string> yossiLogin = _market.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result editItemResult = _market.EditItemInStore(yossiLogin.Value,
                new ItemDto(name, storeName, amount, category, Array.AsReadOnly(tags), price));
            Assert.True(editItemResult.IsSuccess, "failed to edit item: " + editItemResult.Error);
            token = _market.Logout(yossiLogin.Value).Value;
            _market.Disconnect(token);
        }
        /*
         * UC - Update product stock- add items
         * UC - Update product stock- subtract items
         * UC - Update existing product's details
         * Req - 4.1
         */
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
        [Test] //TODO add test for an existing store that isn't owned by the user
        public void TestEditItemInStoreFailureInvalid(string name, string storeName, int amount, string category, string[] tags,
            double price)
        {
            string token = _market.Connect();
            Result<string> yossiLogin = _market.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result editItemResult = _market.EditItemInStore(yossiLogin.Value,
                new ItemDto(name, storeName, amount, category, Array.AsReadOnly(tags), price));
            Assert.True(editItemResult.IsFailure, "was suppose to fail to edit item");
            token = _market.Logout(yossiLogin.Value).Value;
            _market.Disconnect(token);
        }
        /*
         * UC - Update product stock- add items
         * UC - Update product stock- subtract items
         * UC - Update existing product's details
         * Req - 4.1
         */
        [TestCase("Tara milk", "Yossi's Store", 10, "dairy",
            new string[]{"dairy", "milk", "Tara"}, (double)6.2)] // edit price (will fail due to wrong steps)
        [Test]
        public void TestEditItemInStoreFailureLogic(string name, string storeName, int amount, string category, string[] tags,
            double price)
        {
            string token = _market.Connect();
            Result editItemResult = _market.EditItemInStore(token,
                new ItemDto(name, storeName, amount, category, Array.AsReadOnly(tags), price));
            Assert.True(editItemResult.IsFailure, "should not succeed since no user is logged in");
            _market.Disconnect(token);
        }
        /*
         * UC - Remove product from store
         * Req - 4.1
         */
        [TestCase("Yossi's Store", "Tara milk")]
        public void TestRemoveProductFromStoreSuccess(string storeName, string productName)
        {
            string token = _market.Connect();
            Result<string> yossiLogin = _market.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result removeItemResult = _market.RemoveProductFromStore(yossiLogin.Value, storeName, productName);
            Assert.True(removeItemResult.IsSuccess, "failed to remove item " + productName + ": " + removeItemResult.Error);
            token = _market.Logout(yossiLogin.Value).Value;
            _market.Disconnect(token); 
        }
        /*
         * UC - Remove product from store
         * Req - 4.1
         */
        [TestCase("Yossi's Store", "Gans 356 air")]
        [TestCase("dancing doors", "Tara milk")] // non existing store
        [Test]      //TODO add an existing store of which the user is not an owner
        public void TestRemoveProductFromStoreFailureInvalid(string storeName, string productName)
        {
            string token = _market.Connect();
            Result<string> yossiLogin = _market.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result removeItemResult = _market.RemoveProductFromStore(yossiLogin.Value, storeName, productName);
            Assert.True(removeItemResult.IsFailure, "product removal was suppose to fail");
            token = _market.Logout(yossiLogin.Value).Value;
            _market.Disconnect(token); 
        }
        /*
         * UC - Remove product from store
         * Req - 4.1
         */
        [TestCase("Yossi's Store", "Tara milk")]
        [Test]      //TODO add an existing store of which the user is not an owner
        public void TestRemoveProductFromStoreFailureLogic(string storeName, string productName)
        {
            string token = _market.Connect();
            Result removeItemResult = _market.RemoveProductFromStore(token, storeName, productName);
            Assert.True(removeItemResult.IsFailure, "product removal was suppose to fail due to user not being logged in");
            _market.Disconnect(token); 
        }
        /*
         * UC - Appoint user to be store co-owner
         * Req - 4.3
         */
        [TestCase("Yossi's Store", "singerMermaid")]
        [TestCase("Yossi's Store", "Liorwork")]
        [Test]
        public void TestAppointCoOwnerSuccess(string storeName, string username)
        {
            string token = _market.Connect();
            Result<string> yossiLogin = _market.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result result = _market.AppointCoOwner(yossiLogin.Value, storeName, username);
            Assert.True(result.IsSuccess, "failed to appoint " + username + ": " + result.Error);
            token = _market.Logout(yossiLogin.Value).Value;
            _market.Disconnect(token);
        }
        /*
         * UC - Appoint user to be store co-owner
         * Req - 4.3
         */
        [TestCase("Yossi11", "qwerty123", "The Polite Frog", "singerMermaid")]
        [TestCase("Yossi11",  "qwerty123", "Yossi's Store", "Yossi11")] //TODO check with other co-owners too
        [TestCase("Yossi11",   "qwerty123", "Yossi's Store", "Tamir123")]
        [TestCase("singerMermaid", "130452abc", "Yossi's Store", "Liorwork")]
        [Test]
        public void TestAppointCoOwnerFailureInvalid(string appointer, string appointerPassword,  string storeName, string username)
        {
            string token = _market.Connect();
            Result<string>login = _market.Login(token, appointer, appointerPassword, ServiceUserRole.Member);
            Result result = _market.AppointCoOwner(login.Value, storeName, username);
            Assert.True(result.IsFailure, "Appointing " + username + " was expected to fail!");
            token = _market.Logout(login.Value).Value;
            _market.Disconnect(token);
        }
        /*
         * UC - Appoint user to be store co-owner
         * Req - 4.3
         */
        [TestCase("Yossi's Store", "singerMermaid")]
        [TestCase("Yossi's Store", "Liorwork")]
        [Test]
        public void TestAppointCoOwnerFailureLogic(string storeName, string username)
        {
            string token = _market.Connect();
            Result result = _market.AppointCoOwner(token, storeName, username);
            Assert.True(result.IsFailure, "Appointing " + username + " was expected to fail since the user wasn't logged in!");
            _market.Disconnect(token);
        }
        /*
         * UC - Appoint Manager
         * Req - 4.5
         */
        [TestCase("Yossi's Store", "singerMermaid")]
        [TestCase("Yossi's Store", "Liorwork")]
        [Test]
        public void TestAppointManagerSuccess(string storeName, string username)
        {
            string token = _market.Connect();
            Result<string> yossiLogin = _market.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result result = _market.AppointManager(yossiLogin.Value, storeName, username);
            Assert.True(result.IsSuccess, "failed to appoint " + username + ": " + result.Error);
            token = _market.Logout(yossiLogin.Value).Value;
            _market.Disconnect(token);
        }
        /*
         * UC - Appoint Manager
         * Req - 4.5
         */
        [TestCase("Yossi11", "qwerty123", "The Polite Frog", "singerMermaid")]
        [TestCase("Yossi11",  "qwerty123", "Yossi's Store", "Yossi11")]
        [TestCase("Yossi11",   "qwerty123", "Yossi's Store", "Tamir123")]
        [TestCase("singerMermaid", "130452abc", "Yossi's Store", "Liorwork")]
        [Test]
        public void TestAppointManagerFailureInvalid(string appointer, string appointerPassword,  string storeName, string username)
        {
            string token = _market.Connect();
            Result<string>login = _market.Login(token, appointer, appointerPassword, ServiceUserRole.Member);
            Result result = _market.AppointManager(login.Value, storeName, username);
            Assert.True(result.IsFailure, "Appointing " + username + " was expected to fail!");
            token = _market.Logout(login.Value).Value;
            _market.Disconnect(token);
        }
        /*
         * UC - Appoint Manager
         * Req - 4.5
         */
        [TestCase("Yossi's Store", "singerMermaid")]
        [TestCase("Yossi's Store", "Liorwork")]
        [Test]
        public void TestAppointManagerFailureLogic(string storeName, string username)
        {
            string token = _market.Connect();
            Result result = _market.AppointManager(token, storeName, username);
            Assert.True(result.IsFailure, "Appointing " + username + " was expected to fail since the user wasn't logged in!");
            _market.Disconnect(token);
        }
        
        //TODO add permission tests once permissions are known
        
        /*
         * UC - Store Owner requests for purchase history for the store
         * Req - 4.11
         */
        [TestCase("Yossi's Store", "Yossi11", "qwerty123")]
        [Test]
        public void TestGetPurchaseHistoryOfStoreSuccess(string storeName, string owner, string password)
        {
            string token = _market.Connect();
            Result<string> login = _market.Login(token, owner, password, ServiceUserRole.Member);
            Result result = _market.GetPurchaseHistoryOfStore(login.Value, storeName);
            Assert.True(result.IsSuccess, "failed to get purchase history of " + storeName + ": " + result.Error);
            token = _market.Logout(login.Value).Value;
            _market.Disconnect(token);  
        }
        /*
         * UC - Store Owner requests for purchase history for the store
         * Req - 4.11
         */
        [TestCase("dancing dragon", "Yossi11", "qwerty123")]
        [TestCase("Yossi's Store", "singerMermaid", "130452abc")]
        [Test]
        public void TestGetPurchaseHistoryOfStoreFailure(string storeName, string owner, string password)
        {
            string token = _market.Connect();
            Result<string> login = _market.Login(token, owner, password, ServiceUserRole.Member);
            Result result = _market.GetPurchaseHistoryOfStore(login.Value, storeName);
            Assert.True(result.IsFailure, "getting purchase history for " + storeName + " was suppose to fail!");
            token = _market.Logout(login.Value).Value;
            _market.Disconnect(token);  
        }
        /*
         * UC - Save items in a shopping cart
         * Req - 2.7
         */
        [TestCase("Tara milk", "Yossi's store", 3)]
        [TestCase("Tara milk", "Yossi's store", 10)]
        [Test]
        public void TestAddItemToCartSuccess(string itemId, string storeName, int amount)
        { 
            string token = _market.Connect();
            Result<string> login = _market.Login(token, "singerMermaid", "130452abc", ServiceUserRole.Member);
            Result result = _market.AddItemToCart(login.Value, itemId, storeName, amount);
            Assert.True(result.IsSuccess, "failed to add  " + " from " + storeName + " to cart: " + result.Error);
            token = _market.Logout(login.Value).Value;
            _market.Disconnect(token);
        }
        /*
         * UC - Save items in a shopping cart
         * Req - 2.7
         */
        [TestCase("Tnuva cream cheese", "Yossi's store", 3)]
        [TestCase("Tara milk", "Yossi's store", -3)]
        [Test] //TODO check what the expected result for adding more items to the cart than the store has in stock
        public void TestAddItemToCartFailure(string itemId, string storeName, int amount)
        { 
            string token = _market.Connect();
            Result<string> login = _market.Login(token, "singerMermaid", "130452abc", ServiceUserRole.Member);
            Result result = _market.AddItemToCart(login.Value, itemId, storeName, amount);
            Assert.True(result.IsFailure, "action was suppose to fail! " + itemId);
            token = _market.Logout(login.Value).Value;
            _market.Disconnect(token);
        }
        /*
         * UC - Edit shopping cart
         * Req - 2.8 
         */
        [TestCase("Tara milk", "Yossi's store", 9)]
        [TestCase("Tara milk", "Yossi's store", 3)]
        [TestCase("Tara milk", "Yossi's store", 0)]
        [TestCase("Tara milk", "Yossi's store", -6)]
        [Test]
        public void EditItemAmountOfCart(string itemId, string storeName, int amount)
        {
            string token = _market.Connect();
            Result<string> login = _market.Login(token, "singerMermaid", "130452abc", ServiceUserRole.Member);
            Result result = _market.AddItemToCart(login.Value, itemId, storeName, 5);
            result = _market.EditItemAmountOfCart(login.Value, itemId, storeName, amount);
            Assert.True(result.IsSuccess, "failed to edit item: " + result.Error);
            token = _market.Logout(login.Value).Value;
            _market.Disconnect(token);
        }
        /*
         * UC - Edit shopping cart
         * Req - 2.8 
         */
        [TestCase("Tnuva cream cheese", "Yossi's store", 3)]
        [TestCase("Tara milk", "dancing dragon", 0)]
        //TODO recheck this [TestCase("Tara milk", "Yossi's store", 15)]
        [Test]
        public void EditItemAmountOfCartFailure(string itemId, string storeName, int amount)
        {
            string token = _market.Connect();
            Result<string> login = _market.Login(token, "singerMermaid", "130452abc", ServiceUserRole.Member);
            Result result = _market.AddItemToCart(login.Value, itemId, storeName, 5);
            result = _market.EditItemAmountOfCart(login.Value, itemId, storeName, amount);
            Assert.True(result.IsFailure, "item changes were suppose to fail");
            token = _market.Logout(login.Value).Value;
            _market.Disconnect(token);
        }
        /*
         *
         * //TODO find good tests 
         */
        //public void TestGetCart(){}
        /*
         *
         * 
         */
       // public void TestGetPurchaseCartPrice() {}
       /*
        *
        * 
        */
       //[Test] //TODO complete
       //public void TestPurchaseCart() {}
       /*
        * UC - Open a store
        * Req - 3.2
        */
       [TestCase("Yossi11", "qwerty123","Yossi's store jr")]
       [TestCase("singerMermaid", "130452abc", "dancing dragon")]
       [Test]
       public void TestOpenStoreSuccess(string member, string password, string storeName)
       { string token = _market.Connect();
           Result<string> login = _market.Login(token, member, password, ServiceUserRole.Member);
           //Result result = _market.OpenStore(login.Value, storeName);
           //Assert.True(result.IsSuccess, result.Error);
           _market.Logout(login.Value);
           _market.Disconnect(token);
       }
       /*
        * UC - Open a store
        * Req - 3.2
        */
       [TestCase("Yossi11", "qwerty123","~~~Yossi's store jr")]
       [TestCase("singerMermaid", "130452abc", "Yossi's Store")]
       [Test]
       public void TestOpenStoreFailure(string member, string password, string storeName)
       { string token = _market.Connect();
           Result<string> login = _market.Login(token, member, password, ServiceUserRole.Member);
          // Result result = _market.OpenStore(login.Value, storeName);
           //Assert.True(result.IsFailure);
           _market.Logout(login.Value);
           _market.Disconnect(token);
       }
       /*
        * UC - Open a store
        * Req - 3.2
        */
       [TestCase("Yossi's Store jr")]
       [Test]
       public void TestOpenStoreFailureGuest(string storeName)
       { string token = _market.Connect();
           // Result result = _market.OpenStore(token, storeName);
           //Assert.True(result.IsFailure);
           _market.Disconnect(token);
       }
       /*
        * UC - Review purchase history
        * Req - 3.7
        */
       [TestCase("Yossi11", "qwerty123")]
       [TestCase("singerMermaid", "130452abc")]
       [Test]
       public void TestGetPurchaseHistorySuccess(string member, string password)
       { string token = _market.Connect();
           Result<string> login = _market.Login(token, member, password, ServiceUserRole.Member);
           Result result = _market.GetPurchaseHistory(login.Value);
           Assert.True(result.IsSuccess, result.Error);
           _market.Logout(login.Value);
           _market.Disconnect(token);
       }
       /*
        * UC - Admin requests for user's purchase history 
        * Req- 6.4
        */
       [Test]
       public void TestAdminGetPurchaseHistorySuccess(string admin, string password,string member)
       { string token = _market.Connect();
           Result<string> login = _market.Login(token, admin, password, ServiceUserRole.Admin);
           Result result = _market.AdminGetPurchaseHistoryUser(login.Value,member);
           Assert.True(result.IsSuccess, result.Error);
           _market.Logout(login.Value);
           _market.Disconnect(token);
       }
       /*
        * UC - Admin requests for user's purchase history 
        * Req- 6.4
        */
       public void TestAdminGetPurchaseHistoryFailure(string admin, string password,string member)
       { string token = _market.Connect();
           Result<string> login = _market.Login(token, admin, password, ServiceUserRole.Admin);
           Result result = _market.AdminGetPurchaseHistoryUser(login.Value,member);
           Assert.True(result.IsFailure);
           _market.Logout(login.Value);
           _market.Disconnect(token);
       }
       /*
        * UC - Admin requests for store history  
        * Req- 6.4
        */
       [Test]
       public void TestAdminGetPurchaseHistoryStoreSuccess(string admin, string password, string store)
       { string token = _market.Connect();
           Result<string> login = _market.Login(token, admin, password, ServiceUserRole.Admin);
           Result result = _market.AdminGetPurchaseHistoryStore(login.Value,store);
           Assert.True(result.IsSuccess, result.Error);
           _market.Logout(login.Value);
           _market.Disconnect(token);
       }
       /*
        * UC - Admin requests for store history  
        * Req- 6.4
        */
       [Test]
       public void TestAdminGetPurchaseHistoryStoreFailure(string admin, string password, string store)
       { string token = _market.Connect();
           Result<string> login = _market.Login(token, admin, password, ServiceUserRole.Admin);
           Result result = _market.AdminGetPurchaseHistoryStore(login.Value,store);
           Assert.True(result.IsFailure);
           _market.Logout(login.Value);
           _market.Disconnect(token);
       }
    }
}