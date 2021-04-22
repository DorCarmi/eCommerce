using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce;
using eCommerce.Business;
using NUnit.Framework;
using Tests.Business.Mokups;

namespace Tests
{
    public class UserTest
    {
        [SetUp]
        public void Setup()
        {
        }

        
        //possible Integration test too
        [Test]
        public void TestOpenStore()
        {
            User user1 = new User(new MemberInfo("Ja Morant", "ja@mail.com", "Ja", DateTime.Now, "Memphis"));
            mokStore store1= new mokStore("BasketBall stuff.. buy here");
            user1.OpenStore(store1);
            Assert.True(user1.HasPermission(store1,StorePermission.ControlStaffPermission).IsSuccess);

            User user2 = new User("10-Day-Contract guy");
            mokStore store2 = new mokStore("More(!) BasketBall stuff.. buy here");
            Assert.False(user2.OpenStore(store2).IsSuccess);
            Assert.False(user2.HasPermission(store2,StorePermission.ControlStaffPermission).IsSuccess);
            
        } [Test]
        public void TestHasPermissions()
        {
            User user1 = new User(new MemberInfo("Ja Morant", "ja@mail.com", "Ja", DateTime.Now, "Memphis"));
            mokStore store1= new mokStore("BasketBall stuff.. buy here");
            user1.OpenStore(store1);
            Assert.True(user1.HasPermission(store1,StorePermission.ControlStaffPermission).IsSuccess);
            User user2 = new User(new MemberInfo("Jaren Jackson Jr.", "jjj@mail.com", "Jaren", DateTime.Now, "Memphis"));
            Assert.False(user2.HasPermission(store1,StorePermission.ControlStaffPermission).IsSuccess);
            user1.AppointUserToManager(store1, user2);
            user1.UpdatePermissionsToManager(store1,user2, new List<StorePermission>(new [] {StorePermission.ChangeItemPrice,StorePermission.ControlStaffPermission}));
            Assert.True(user2.HasPermission(store1, StorePermission.ChangeItemPrice).IsSuccess);
            Assert.True(user2.HasPermission(store1, StorePermission.ControlStaffPermission).IsSuccess);
            Assert.False(user2.HasPermission(store1, StorePermission.EditItemDetails).IsSuccess);

        }
        [Test]
        public void TestAppointUserToManager()
        {
            User user1 = new User(new MemberInfo("Ja Morant", "ja@mail.com", "Ja", DateTime.Now, "Memphis"));
            mokStore store1= new mokStore("BasketBall stuff.. buy here");
            user1.OpenStore(store1);
            
            User user2 = new User(new MemberInfo("Jaren Jackson Jr.", "jjj@mail.com", "Jaren", DateTime.Now, "Memphis"));
            Assert.False(user2.AppointUserToManager(store1, user1).IsSuccess);
            Assert.True(user1.AppointUserToManager(store1, user2).IsSuccess);
            Assert.True(user1.AppointedManagers != null);
            Assert.True(user1.AppointedManagers.ContainsKey(store1));
            Assert.True(user1.AppointedManagers[store1].FirstOrDefault(m => m.User == user2)!= null);
            
            
            User user3 = new User("10-Day-Contract guy");
            Assert.False(user1.AppointUserToManager(store1,user2).IsSuccess);
            Assert.False(user1.AppointUserToManager(store1,user3).IsSuccess);
        }

        [Test]
        public void TestUpdatePermissionsToManager()
        {
            User user1 = new User(new MemberInfo("Ja Morant", "ja@mail.com", "Ja", DateTime.Now, "Memphis"));
            User user2 = new User(new MemberInfo("Jaren Jackson Jr.", "jjj@mail.com", "Jaren", DateTime.Now, "Memphis"));
            mokStore store1= new mokStore("BasketBall stuff.. buy here");
            user1.OpenStore(store1);
            Assert.False(user1.UpdatePermissionsToManager(store1,user2,new List<StorePermission>(new [] {StorePermission.AddItemToStore})).IsSuccess);
            Assert.True(user1.AppointUserToManager(store1,user2).IsSuccess);
            Assert.False(user1.UpdatePermissionsToManager(store1,user2,new List<StorePermission>()).IsSuccess);
            Assert.True(user1.UpdatePermissionsToManager(store1,user2,new List<StorePermission>(new [] {StorePermission.AddItemToStore})).IsSuccess);
            Assert.True(user2.StoresManaged[store1].HasPermission(StorePermission.AddItemToStore).IsSuccess);
        }

        [Test]
        public void TestUserPurchaseHistory()
        {
            User user1 = new User(new MemberInfo("Ja Morant", "ja@mail.com", "Ja", DateTime.Now, "Memphis"));
            mokStore store1= new mokStore("BasketBall stuff.. buy here");
            var result = user1.GetUserPurchaseHistory();
            Assert.True(result.IsSuccess);
            var records = result.Value;
            Assert.True(records.Count == 0);
            var purchaseRecord = new PurchaseRecord(store1,new MokBasket(user1.GetCartInfo().Value,store1),DateTime.Now);
            Assert.True(user1.EnterRecordToHistory(purchaseRecord).IsSuccess);
            result = user1.GetUserPurchaseHistory();
            Assert.True(result.IsSuccess);
            Assert.True(result.Value.Contains(purchaseRecord));
        }

        [Test]
        public void TestAdminGetHistory()
        {
            User user1 = new User(Admin.State,new MemberInfo("GM", "gm@mail.com", "GM", DateTime.Now, "Memphis"));
            User user2 = new User(new MemberInfo("Ja Morant", "ja@mail.com", "Ja", DateTime.Now, "Memphis"));
            mokStore store1= new mokStore("BasketBall stuff.. buy here");
            var purchaseRecord = new PurchaseRecord(store1,new MokBasket(user2.GetCartInfo().Value,store1),DateTime.Now);
            user2.EnterRecordToHistory(purchaseRecord);
            var result = user1.GetUserPurchaseHistory(user2);
            Assert.True(result.IsSuccess);
            User user3 = new User("10-Day-Contract guy");
            Assert.False(user1.GetUserPurchaseHistory(user3).IsSuccess);
            Assert.False(user2.GetUserPurchaseHistory(user1).IsSuccess);
        }

        
    }
}