using System;
using System.Collections.Generic;
using eCommerce.Business;
using eCommerce.Business.Service;
using eCommerce.Common;
using NUnit.Framework;

namespace Tests.Business
{
    [TestFixture]
    public class UserStoreIntegration
    {
        private const string STORE_NAME = "The store";
        
        private IUser _user;
        private IStore _store;
        private List<ItemInfo> _itemInfos;

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
        
        
        [SetUp]
        public void Setup()
        {
            MemberInfo memberInfo = new MemberInfo("User1", "email@email.com", "TheUser", DateTime.Now, "The sea 1");
            _user = new User(Member.State, memberInfo);
            _store = new Store(STORE_NAME, _user);
            _user.OpenStore(_store);

        }
        
        [Test]
        public void AddItemToStoreTest()
        {
            for (int i = 1; i < _itemInfos.Count; i++)
            {
                Result itemAdditionRes = _store.AddItemToStore(_itemInfos[i], _user);
                Assert.True(itemAdditionRes.IsSuccess,
                    $"Item {_itemInfos[i].name} wasn't added to store\nError: {itemAdditionRes.Error}");
            }
        }
        
        [Test]
        public void AddItemToCartTest()
        {
            Result<Item> itemRes = _store.GetItem(_itemInfos[0].name);
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
            
            Result addItemRes = _user.AppointUserToManager(_store, newUser);
            Assert.True(addItemRes.IsSuccess,
                $"Error {addItemRes.Error}");
        }
        
        [Test]
        public void SearchItemByCategoryTest()
        {
            AddItemToStoreTest();
            IList<Item> searchRes = _store.SearchItemWithCategoryFilter("Orange", _itemInfos[2].category);
            Assert.AreEqual(1, searchRes.Count);
        }
    }
}