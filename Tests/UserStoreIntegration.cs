using System;
using System.Collections.Generic;
using eCommerce.Business;
using eCommerce.Business.Service;
using eCommerce.Common;
using NUnit.Framework;

namespace Tests
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
                    "fruit", new List<string>() {"Sugar"}, 20),
                new ItemInfo(100, "Water", STORE_NAME,
                    "fruit", new List<string>() {"Drink"}, 20)
            };
        }
        
        
        [SetUp]
        public void Setup()
        {
            MemberInfo memberInfo = new MemberInfo("User1", "email@email.com", "TheUser", DateTime.Now, "The sea 1");
            _user = new User(Member.State, memberInfo);
            _store = new Store(STORE_NAME, _user, _itemInfos[0]);

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
    }
}