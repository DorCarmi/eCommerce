

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using eCommerce.Business;
using eCommerce.Business.Service;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;
using Tests.Business.Mokups;

namespace Tests.Business.StoreTests
{
    public class StoreTest
    {
        private IStore MyStore;
        private IUser Alice;
        private IUser Bob;
        private ItemInfo item1;
        private ItemInfo item1b;
        private ItemInfo item2;
        public StoreTest()
        {
            Alice = new mokUser("Alice");
            Bob = new mokUser("Bob");
            this.MyStore = new Store("Alenby", Alice);
        }
        
        [SetUp]
        public void Setup()
        {
            
            item1 = new ItemInfo(50, "IPhone", "Alenby", "Tech", new List<string>(), 5000);
            item1.AssignStoreToItem(MyStore);
            item1b = new ItemInfo(50, "IPhone", "Alenby", "Computers", new List<string>(){"A"}, 3000);
            item1b.AssignStoreToItem(MyStore);
            item2 = new ItemInfo(50, "Dell6598", "Alenby", "Tech", new List<string>(), 10000);
            item2.AssignStoreToItem(MyStore);
        }

        [Test]
        public void TestUsersInStore()
        {
            ManagerAppointment managerAppointment = new ManagerAppointment(Bob);
            OwnerAppointment ownerAppointment = new OwnerAppointment(Bob);
            var manager = this.MyStore.AppointNewManager(Alice, managerAppointment);
            var owner = this.MyStore.AppointNewOwner(Alice, ownerAppointment);
            
            Assert.AreEqual(true,manager.IsSuccess);
            Assert.AreEqual(true,owner.IsSuccess);

            var staff=MyStore.GetStoreStaffAndTheirPermissions(Alice);
            Assert.AreEqual(true,staff.IsSuccess);

            bool AliceInStore =
                staff.Value.FirstOrDefault(x => x.Item1.Equals(Alice.Username)) != null;
            bool BobInStore =
                staff.Value.FirstOrDefault(x => x.Item1.Equals(Bob.Username)) != null;
            
            Assert.AreEqual(true,AliceInStore);
            Assert.AreEqual(true,BobInStore);
            //MyStore.GetStoreStaffAndTheirPermissions()
        }
        
        [Test]
        public void TestItemsInStore()
        {
            Assert.AreEqual(0,MyStore.GetAllItems().Count);
            var addRes1 = MyStore.AddItemToStore(item1, Alice);
            Assert.AreEqual(true,addRes1.IsSuccess);
            var AllItems = MyStore.GetAllItems();
            Assert.AreNotEqual(null,AllItems.FirstOrDefault(x=>x.GetName().Equals(item1.name)));

            var item1res=MyStore.GetItem(item1);
            Assert.AreEqual(true,item1res.IsSuccess);
            Assert.AreEqual(item1.name, item1res.Value.GetName());
            Assert.AreEqual(item1.Amount, item1res.Value.GetAmount());
            Assert.AreEqual(item1.category, item1res.Value.GetCategory().getName());
            Assert.AreEqual(item1.pricePerUnit, item1res.Value.GetPricePerUnit());

            var item1res2=MyStore.GetItem(item1.name);
            Assert.AreEqual(true,item1res2.IsSuccess);
            Assert.AreEqual(item1.name, item1res2.Value.GetName());
            Assert.AreEqual(item1.Amount, item1res2.Value.GetAmount());
            Assert.AreEqual(item1.category, item1res2.Value.GetCategory().getName());
            Assert.AreEqual(item1.pricePerUnit, item1res2.Value.GetPricePerUnit());

            var searchRes=MyStore.SearchItem(item1.name);
            Assert.AreNotEqual(null,searchRes.FirstOrDefault(x=>x.GetName().Equals(item1.name)));
            Assert.AreNotEqual(null,searchRes.FirstOrDefault(x=>x.GetCategory().getName().Equals(item1.category)));
            Assert.AreNotEqual(null,searchRes.FirstOrDefault(x=>x.GetPricePerUnit().Equals(item1.pricePerUnit)));
            Assert.AreNotEqual(null,searchRes.FirstOrDefault(x=>x.GetAmount().Equals(item1.amount)));

            
            var resEdit=MyStore.EditItemToStore(item1b, Alice);
            Assert.AreEqual(true,resEdit.IsSuccess);
            var item1resEdit=MyStore.GetItem(item1.name);
            Assert.AreEqual(true,item1res2.IsSuccess);
            Assert.AreEqual(item1b.name, item1res2.Value.GetName());
            Assert.AreEqual(item1b.Amount, item1res2.Value.GetAmount());
            Assert.AreEqual(item1b.category, item1res2.Value.GetCategory().getName());
            Assert.AreEqual(item1b.pricePerUnit, item1res2.Value.GetPricePerUnit());
            Assert.AreNotEqual(0,item1b.keyWords.Count);

            //MyStore.EditItemToStore()
            //MyStore.RemoveItemToStore()
            //MyStore.SearchItemWithCategoryFilter()
            //MyStore.SearchItemWithPriceFilter()
        }
        
        public void TestItemsStock()
        {
            //MyStore.UpdateStock_AddItems()
            //MyStore.UpdateStock_SubtractItems()
        }
        
        public void TestPurchaseProcess()
        {
            //MyStore.CatchAllBasketProducts()
            //MyStore.CalculateBasketPrices()
            //MyStore.FinishPurchaseOfBasket()
            //MyStore.FinishPurchaseOfItems()
            //MyStore.CheckDiscount()
        }

        public void TestStoreInfoAndPolicy()
        {
            //MyStore.GetStoreName()
            //MyStore.CheckWithPolicy()
            //MyStore.UpdatePurchaseStrategies()
            //MyStore.AddPurchaseStrategyToStore()
            //MyStore.AddPurchaseStrategyToStoreItem()
            //MyStore.GetPurchaseStrategyToStoreItem()
            //MyStore.RemovePurchaseStrategyToStoreItem()
            
        }

        public void TestBasketsInStore()
        {
            //MyStore.ConnectNewBasketToStore()
            //MyStore.CheckConnectionToCart()
            //MyStore.AddBasketToStore()
            //MyStore.CalculateBasketPrices()
            //MyStore.GetStorePurchaseStrategy()
            //MyStore.TryAddNewCartToStore()
        }

        public void TestHistory()
        {
            //MyStore.GetPurchaseHistory()
            //MyStore.EnterBasketToHistory()
        }
        
        

        [TearDown]
        public void TearDown()
        {

        }
    }
}