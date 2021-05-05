using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Business;
using eCommerce.Business.Service;
using eCommerce.Common;
using NUnit.Framework;
using Tests.Business.Mokups;

namespace Tests.Business
{
    [TestFixture]
    public class UserStoreIntegration
    {
        private const string STORE_NAME = "The store";
        
        private IUser _user;
        private List<ItemInfo> _itemInfos;
        
        private IUser alenbyFounder;
        private IUser alenbyOwner;
        private IUser alenbyCoOwner;
        private IUser alenbyManager;
        private IUser alenbySecondManager;
        private Store alenbyStore;

        public UserStoreIntegration()
        {
            _itemInfos = new List<ItemInfo>()
            {
                new ItemInfo(10, "Watermellon", STORE_NAME,
                    "fruit", new List<string>() {"Watermellon"}, 20),
                new ItemInfo(5, "Cream", STORE_NAME,
                    "Sweat", new List<string>() {"Sugar"}, 20),
                new ItemInfo(100, "Water", STORE_NAME,
                    "Drink", new List<string>() {"Drink"}, 20),
                new ItemInfo(20, "Orange juice", STORE_NAME,
                    "Drink", new List<string>() {"Drink"}, 20)
            };
        }
        
        
        [OneTimeSetUp]
        public void Setup()
        {
            MemberInfo memberInfo = new MemberInfo("alenbyFounder", "email@email.com", "TheUser", DateTime.Now, "The sea 1");

            alenbyFounder = new User(new MemberInfo("alenbyFounder", "alenbyFounder@gmail.com", "A", DateTime.Now, "TLV"));
            alenbyManager = new User(new MemberInfo("alenbyManager", "alenbyManager@gmail.com", "A", DateTime.Now, "TLV"));
            alenbyOwner = new User(new MemberInfo("alenbyOwner", "alenbyOwner@gmail.com", "A", DateTime.Now, "TLV"));
            alenbyCoOwner = new User(new MemberInfo("alenbyCoOwner", "alenbyCoOwner@gmail.com", "A", DateTime.Now, "TLV"));
            alenbySecondManager = new User(new MemberInfo("alenbySecondManager", "alenbySecondManager@gmail.com", "A", DateTime.Now, "TLV"));
            
            alenbyStore = new Store("Alenby",alenbyFounder);
            alenbyFounder.OpenStore(alenbyStore);
        }

        [Test]
        [Order(0)]
        public void OpenStoreTest()
        {
            Store store = new Store("Fox", alenbyFounder);
            var res=this.alenbyFounder.OpenStore(store);
            Assert.True(res.IsSuccess);
        }
        
        [Test]
        [Order(1)]
        public void AppointOwnerTest()
        {
            var res= alenbyFounder.AppointUserToOwner(alenbyStore,alenbyCoOwner);
            Assert.AreEqual("",res.Error);
            Assert.True(res.IsSuccess);
        }
        
        [Test]
        [Order(2)]
        public void AppointManagerTest()
        {
            //alenbyFounder.AppointUserToOwner(alenbyStore,alenbyCoOwner);
            var task1 = new Task<Result>( ()=> alenbyFounder.AppointUserToManager(alenbyStore, alenbyManager));
            var task2 = new Task<Result>( ()=> alenbyCoOwner.AppointUserToManager(alenbyStore, alenbyManager));
            task1.Start();
            task2.Start();
            Assert.True(task1.Result.IsSuccess != task2.Result.IsSuccess);
        }

        [Test]
        public void PurchaseTest()
        {
            var pstation = new ItemInfo(100, "Playstation4", alenbyStore.GetStoreName(), "Tech",
                new List<string>(), 3500);
           var addItemRes= alenbyStore.AddItemToStore(pstation,alenbyFounder);
           
           Assert.True(addItemRes.IsSuccess);

           var resGetItem=alenbyStore.GetItem(pstation);
           var showItem = resGetItem.Value.ShowItem();
           showItem.amount = 5;
           var resAddToCart=alenbySecondManager.AddItemToCart(showItem);
           Assert.AreEqual("",resAddToCart.Error);
           var purchaseRes=alenbySecondManager.BuyWholeCart(new PaymentInfo(
               userName:alenbySecondManager.Username,
               idNumber:alenbySecondManager.MemberInfo.Id,
               creditCardNumber:"1234567789",
               creditCardExpirationDate:"0322",
               threeDigitsOnBackOfCard:"123",
               fullAddress:"TLV"
               ));
           
           Assert.AreEqual("",purchaseRes.Error);
        }
        
        
        
        [Test]
        public void AddItemToStoreTest()
        {
            for (int i = 1; i < _itemInfos.Count; i++)
            {
                Result itemAdditionRes = alenbyStore.AddItemToStore(_itemInfos[i], _user);
                Assert.True(itemAdditionRes.IsSuccess,
                    $"Item {_itemInfos[i].name} wasn't added to store\nError: {itemAdditionRes.Error}");
            }
        }
        
        [Test]
        public void AddItemToCartTest()
        {
            Result<Item> itemRes = alenbyStore.GetItem(_itemInfos[0].name);
            Assert.True(itemRes.IsSuccess, $"{itemRes.Error}");
            
            Result addItemRes = _user.AddItemToCart(itemRes.Value.ShowItem());
            Assert.True(addItemRes.IsSuccess,
                $"Error {addItemRes.Error}");
        }
        
        [Test]
        public void AppointNewMangerTest()
        {
            MemberInfo memberInfo = new MemberInfo("User2", "User2@email.com", "TheUser1", DateTime.Now, "The sea 3");
            IUser newUser = new User(Member.State, memberInfo);
            
            Result addItemRes = _user.AppointUserToManager(alenbyStore, newUser);
            Assert.True(addItemRes.IsSuccess,
                $"Error {addItemRes.Error}");
        }
        
        [Test]
        public void SearchItemByCategoryTest()
        {
            AddItemToStoreTest();
            IList<Item> searchRes = alenbyStore.SearchItemWithCategoryFilter("Orange", _itemInfos[2].category);
            Assert.AreEqual(1, searchRes.Count);
        }

        [Test]
        public void AppointUsersTest()
        {
            User alice = new User(new MemberInfo("Alice","email@gmail.com",
                "mycooluser",DateTime.Now,"Rager 16, Beer Sheva" ));
            Store store = new Store("FOX", alice);
            alice.OpenStore(store);

            User bob = new User(new MemberInfo("Bob", "bob@gmail.com",
                "coolbob", DateTime.Now, "Maccabi"));
            var res = alice.AppointUserToManager(store, bob);
            Assert.True(res.IsSuccess);

        }
    }
}