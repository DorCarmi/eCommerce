using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce;
using eCommerce.Business;
using eCommerce.Common;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Tests.Business.Mokups;

namespace Tests.Business.UserTests
{
    public class UserTest
    {
        private User ja;
        private mokStore store1;
        private User tempguy;
        private mokStore store2;
        private User jaren;
        private User GM;
        private User brandon;


        [SetUp]
        public void Setup()
        {
            ja = new User(new MemberInfo("Ja Morant", "ja@mail.com", "Ja", DateTime.Now, "Memphis"));
            store1= new mokStore("BasketBall stuff.. buy here");
            tempguy = new User("10-Day-Contract guy");
            store2 = new mokStore("More(!) BasketBall stuff.. buy here");
            jaren = new User(new MemberInfo("Jaren Jackson Jr.", "jjj@mail.com", "Jaren", DateTime.Now, "Memphis"));
            GM = new User(Admin.State,new MemberInfo("GM", "gm@mail.com", "GM", DateTime.Now, "Memphis"));
            brandon = new User(new MemberInfo("Brandon Clark","bc@mail.com","brandon",DateTime.Now, "Memphis"));
            ja.OpenStore(store1);

        }

        //possible Integration test too
        [Test]
        public void TestOpenStore_Pass()
        {
            Assert.True(ja.HasPermission(store1,StorePermission.ControlStaffPermission).IsSuccess);
            Assert.True(ja.StoresOwned.ContainsKey(store1));
            Assert.True(ja.StoresOwned[store1].User.Equals(ja));
        } 
        [Test]
        public void TestOpenStore_Fail()
        {
            Assert.False(tempguy.OpenStore(store2).IsSuccess);
            Assert.False(tempguy.HasPermission(store2,StorePermission.ControlStaffPermission).IsSuccess);
        }
        [Test]
        public void TestOpenStore_Concurrent()
        {
            var store3 = new mokStore("New Location(!) BasketBall stuff.. buy here");
            var store4 = new mokStore("Old Location Again(!) BasketBall stuff.. buy here");
            Task<Result> task1 = new Task<Result>(() => { return jaren.OpenStore(store2); });
            Task<Result> task2 = new Task<Result>(() => { return jaren.OpenStore(store2); });
            task1.Start();
            task2.Start();
            Assert.True(task1.Result.IsSuccess != task2.Result.IsSuccess);
            Task<Result> task3 = new Task<Result>(() => { return jaren.OpenStore(store3); });
            Task<Result> task4 = new Task<Result>(() => { return jaren.OpenStore(store4); });
            task3.Start();
            task4.Start();
            Assert.True(task3.Result.IsSuccess);
            Assert.True(task4.Result.IsSuccess);
        }
        
        [Test]
        public void TestHasPermissions_Pass()
        {
            Assert.True(ja.HasPermission(store1,StorePermission.ControlStaffPermission).IsSuccess);
            ja.AppointUserToManager(store1, jaren);
            ja.UpdatePermissionsToManager(store1,jaren, new List<StorePermission>(new [] {StorePermission.ChangeItemPrice,StorePermission.ControlStaffPermission}));
            Assert.True(jaren.HasPermission(store1, StorePermission.ChangeItemPrice).IsSuccess);
            Assert.True(jaren.HasPermission(store1, StorePermission.ControlStaffPermission).IsSuccess);
        }
        [Test]
        public void TestHasPermissions_Fail()
        {
            Assert.False(jaren.HasPermission(store1,StorePermission.ControlStaffPermission).IsSuccess);
            ja.AppointUserToManager(store1, jaren);
            ja.UpdatePermissionsToManager(store1,jaren, new List<StorePermission>(new [] {StorePermission.ChangeItemPrice,StorePermission.ControlStaffPermission}));
            Assert.False(jaren.HasPermission(store1, StorePermission.EditItemDetails).IsSuccess);
        }
        [Test]
        public void TestHasPermissions_Concurrent()
        {
            ja.AppointUserToManager(store1,jaren);
            var permissionList = new List<StorePermission>(new[] {StorePermission.ChangeItemPrice, StorePermission.ControlStaffPermission});
            var taskAdd1 = new Task<Result>(() => ja.UpdatePermissionsToManager(store1,jaren,permissionList));
            var taskAdd2 = new Task<Result>(() => ja.UpdatePermissionsToManager(store1,jaren,permissionList));
            var taskAdd3 = new Task<Result>(() => ja.UpdatePermissionsToManager(store1,jaren,permissionList));
            taskAdd1.Start();
            taskAdd2.Start();
            taskAdd3.Start();
            Assert.True(taskAdd1.Result.IsSuccess);
            Assert.True(taskAdd2.Result.IsSuccess);
            Assert.True(taskAdd3.Result.IsSuccess);
            Assert.True(jaren.HasPermission(store1, StorePermission.ChangeItemPrice).IsSuccess);
            Assert.True(jaren.HasPermission(store1, StorePermission.ControlStaffPermission).IsSuccess);
        }
        [Test]
        public void TestAppointUserToManager_Pass()
        {
            Assert.True(ja.AppointUserToManager(store1, jaren).IsSuccess);
            Assert.True(ja.AppointedManagers != null);
            Assert.True(ja.AppointedManagers.ContainsKey(store1));
            Assert.True(ja.AppointedManagers[store1].FirstOrDefault(m => m.User == jaren)!= null);
        }
        [Test]
        public void TestAppointUserToManager_Fail()
        {
            Assert.False(jaren.AppointUserToManager(store1, ja).IsSuccess);
            ja.AppointUserToManager(store1, jaren);
            Assert.False(ja.AppointUserToManager(store1,jaren).IsSuccess);
            Assert.False(ja.AppointUserToManager(store1,tempguy).IsSuccess);
        }
        [Test]
        public void TestAppointUserToManager_Concurrent()
        {
            ja.AppointUserToOwner(store1, jaren);
            var task1 = new Task<Result>( ()=> ja.AppointUserToManager(store1, brandon));
            var task2 = new Task<Result>( ()=> jaren.AppointUserToManager(store1, brandon));
            task1.Start();
            task2.Start();
            Assert.True(task1.Result.IsSuccess != task2.Result.IsSuccess);
            Assert.True(ja.AppointedManagers.ContainsKey(store1) != jaren.AppointedManagers.ContainsKey(store1));
        }
        [Test]
        public void TestAppointUserToOwner_Pass()
        {
            Assert.True(ja.AppointUserToOwner(store1, jaren).IsSuccess);
            Assert.True(ja.AppointedOwners != null);
            Assert.True(ja.AppointedOwners.ContainsKey(store1));
            Assert.True(ja.AppointedOwners[store1].FirstOrDefault(m => m.User == jaren)!= null);
        }
        [Test]
        public void TestAppointUserToOwner_Fail()
        {
            Assert.False(jaren.AppointUserToOwner(store1, ja).IsSuccess);
            ja.AppointUserToOwner(store1, jaren);
            Assert.False(ja.AppointUserToOwner(store1,jaren).IsSuccess);
            Assert.False(ja.AppointUserToOwner(store1,tempguy).IsSuccess);
        }
        [Test]
        public void TestAppointUserToOwner_Concurrent()
        {
            ja.AppointUserToOwner(store1, jaren);
            var task1 = new Task<Result>( ()=> ja.AppointUserToOwner(store1, brandon));
            var task2 = new Task<Result>( ()=> jaren.AppointUserToOwner(store1, brandon));
            task1.Start();
            task2.Start();
            Assert.True(task1.Result.IsSuccess != task2.Result.IsSuccess);
            int OwnersCount = 0;
            OwnersCount += ja.AppointedOwners[store1].Count;
            if (jaren.AppointedOwners.ContainsKey(store1))
            {
                OwnersCount += jaren.AppointedOwners[store1].Count;
            }
            Assert.True(OwnersCount == 2);
        }
        [Test]
        public void TestUpdatePermissionsToManager_Pass()
        {
            Assert.True(ja.AppointUserToManager(store1,jaren).IsSuccess);
            Assert.True(ja.UpdatePermissionsToManager(store1,jaren,new List<StorePermission>(new [] {StorePermission.AddItemToStore})).IsSuccess);
            Assert.True(jaren.StoresManaged[store1].HasPermission(StorePermission.AddItemToStore).IsSuccess);
        }
        [Test]
        public void TestUpdatePermissionsToManager_Fail()
        {
            Assert.False(ja.UpdatePermissionsToManager(store1,jaren,new List<StorePermission>(new [] {StorePermission.AddItemToStore})).IsSuccess);
            ja.AppointUserToManager(store1,jaren);
            Assert.False(ja.UpdatePermissionsToManager(store1,jaren,new List<StorePermission>()).IsSuccess);
        }
        [Test]
        public void TestUpdatePermissionsToManager_Concurrent()
        {
            ja.AppointUserToManager(store1,jaren);
            var task1 = new Task<Result>(()=> ja.UpdatePermissionsToManager(store1,jaren,new List<StorePermission>(new [] {StorePermission.AddItemToStore, StorePermission.ChangeItemPrice})));
            var task2 = new Task<Result>(()=> ja.UpdatePermissionsToManager(store1,jaren,new List<StorePermission>(new [] {StorePermission.AddItemToStore, StorePermission.ChangeItemStrategy})));
            task1.Start();
            task2.Start();
            Assert.True(task1.Result.IsSuccess);
            Assert.True(task2.Result.IsSuccess);
            Assert.True(jaren.StoresManaged[store1].HasPermission(StorePermission.AddItemToStore).IsSuccess);
            Assert.True(jaren.StoresManaged[store1].HasPermission(StorePermission.ChangeItemPrice).IsSuccess != jaren.StoresManaged[store1].HasPermission(StorePermission.ChangeItemStrategy).IsSuccess);
        }
        [Test]
        public void TestUserPurchaseHistory_Pass()
        {
            var result = ja.GetUserPurchaseHistory();
            Assert.True(result.IsSuccess);
            var records = result.Value;
            Assert.True(records.Count == 0);
            var purchaseRecord = new PurchaseRecord(store1,new MokBasket(ja.GetCartInfo().Value,store1),DateTime.Now);
            Assert.True(ja.EnterRecordToHistory(purchaseRecord).IsSuccess);
            result = ja.GetUserPurchaseHistory();
            Assert.True(result.IsSuccess);
            Assert.True(result.Value.Contains(purchaseRecord));
        } 
        [Test]
        public void TestUserPurchaseHistory_Fail()
        {
            var result = tempguy.GetUserPurchaseHistory();
            Assert.False(result.IsSuccess);
            var purchaseRecord = new PurchaseRecord(store1,new MokBasket(tempguy.GetCartInfo().Value,store1),DateTime.Now);
            Assert.False(tempguy.EnterRecordToHistory(purchaseRecord).IsSuccess);
            result = tempguy.GetUserPurchaseHistory();
            Assert.False(result.IsSuccess);
        }
        
        [Test]
        public void TestUserPurchaseHistory_Concurrent()
        {
            var purchaseRecord1 = new PurchaseRecord(store1,new MokBasket(ja.GetCartInfo().Value,store1),DateTime.Now);
            var purchaseRecord2 = new PurchaseRecord(store1,new MokBasket(ja.GetCartInfo().Value,store2),DateTime.Now);
            var task1 =  new Task<Result>(()=>ja.EnterRecordToHistory(purchaseRecord1));
            var task2 =  new Task<Result>(()=>ja.EnterRecordToHistory(purchaseRecord2));
            task1.Start();
            task2.Start();
            Assert.True(task1.Result.IsSuccess);
            Assert.True(task2.Result.IsSuccess);
            var result = ja.GetUserPurchaseHistory();
            Assert.True(result.IsSuccess);
            Assert.True(result.Value.Contains(purchaseRecord1));
            Assert.True(result.Value.Contains(purchaseRecord2));
        } 
        [Test]
        public void TestAdminGetHistory_Pass()
        {
            var purchaseRecord = new PurchaseRecord(store1,new MokBasket(ja.GetCartInfo().Value,store1),DateTime.Now);
            ja.EnterRecordToHistory(purchaseRecord);
            var result = GM.GetUserPurchaseHistory(ja);
            Assert.True(result.IsSuccess);
            var records = result.Value;
            Assert.True(records.Count == 1);
        }
        [Test]
        public void TestAdminGetHistory_Fail()
        {
            Assert.False(GM.GetUserPurchaseHistory(tempguy).IsSuccess);
            Assert.False(ja.GetUserPurchaseHistory(GM).IsSuccess);
        }
        [Test]
        public void TestAdminGetHistory_Concurrent()
        {
            var purchaseRecord = new PurchaseRecord(store1,new MokBasket(ja.GetCartInfo().Value,store1),DateTime.Now);
            ja.EnterRecordToHistory(purchaseRecord);
            var VP = new User(Admin.State, new MemberInfo("VPofBballOps","vp@mail.com","Vp", DateTime.Now, "Memphis as well"));
            var task1 =new Task<Result<IList<PurchaseRecord>>>(()=> GM.GetUserPurchaseHistory(ja));
            var task2 =new Task<Result<IList<PurchaseRecord>>>(()=> VP.GetUserPurchaseHistory(ja));
            task1.Start();
            task2.Start();
            Assert.True(task1.Result.IsSuccess);
            Assert.True(task2.Result.IsSuccess);
            Assert.True(task1.Result.Value.Count == task2.Result.Value.Count);
        }
        
    }
}