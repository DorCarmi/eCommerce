using System;
using System.Collections.Generic;
using eCommerce.Business;
using eCommerce.Common;
using eCommerce.DataLayer;
using NUnit.Framework;

namespace Tests.DataLayer
{
    /**
     * DB should be clear
     */
    [TestFixture]
    public class DataLayerTest
    {
        private DataFacade df;
        private User ja;
        private User jaren;
        private User dillon;
        private Store store1;
        private Store store2;
        private Store store3;
        private Store store4;
        private Store alenbyStore;
       
        public DataLayerTest()
        {
            df = new DataFacade();
            df.init(true);
        }

        

        [SetUp]
        public void Setup()
        {
            // set up business users and stores
            var info1 = new MemberInfo("Ja Morant", "ja@mail.com", "Ja", DateTime.Now, "Memphis");
            info1.Id = "12";
            var info2 = new MemberInfo("Jaren Jackson Jr.", "jjj@mail.com", "Jaren", DateTime.Now, "Memphis");
            info2.Id = "13";
            var info3 = new MemberInfo("Dillon Brooks", "db@mail.com", "Dillon", DateTime.Now, "Memphis");
            info3.Id = "24";

            ja = new User(info1);
            jaren = new User(info2);
            dillon = new User(info3);
            store1= new Store("BasketBall stuff.. buy here",ja);
            store2= new Store("More(!) BasketBall stuff.. buy here",ja);
            store3= new Store("EVEN MORE!! BasketBall stuff.. buy here",ja);
            store4= new Store("ok we had too much BasketBall stuff.. buy here",ja);
            alenbyStore= new Store("this is the store in alenby.. buy here",ja);

            //clear DB
            df.ClearTables();
        }

        [Test]
        [Order(1)]
        public void SaveUserTest()
        {
            Assert.True(df.SaveUser(ja).IsSuccess);
        }
        
        [Test]
        [Order(2)]
        public void ReadUserTest()
        {
            SaveUserTest();
            Assert.True(df.ResetConnection().IsSuccess);
            Assert.True(df.ReadUser(ja.Username).IsSuccess);
        }
        
        
        [Test]
        public void SaveUserWithStoreTest()
        {
            Assert.True(df.SaveUser(ja).IsSuccess);
            Assert.True(df.SaveUser(jaren).IsSuccess);
            Assert.True(df.SaveStore(store1).IsSuccess);
            Assert.True(df.SaveStore(store2).IsSuccess);
            Assert.True(df.SaveStore(store3).IsSuccess);
            Assert.True(df.SaveStore(store4).IsSuccess);

            ja.OpenStore(store1);
            ja.OpenStore(store2);
            ja.OpenStore(store3);
            ja.OpenStore(store4);
            ja.AppointUserToManager(store1,jaren);
            ja.AppointUserToManager(store2,jaren);
            ja.AppointUserToOwner(store3, jaren);
            ja.AppointUserToOwner(store4, jaren);
            var prems = new List<StorePermission>() {StorePermission.ChangeItemPrice, StorePermission.ControlStaffPermission, StorePermission.EditStorePolicy, StorePermission.AddItemToStore};
            ja.UpdatePermissionsToManager(store2,jaren, prems);
           
            Assert.True(df.UpdateUser(ja).IsSuccess);
            Assert.True(df.UpdateUser(jaren).IsSuccess);
            Assert.True(df.UpdateStore(store1).IsSuccess);
            Assert.True(df.UpdateStore(store3).IsSuccess);
        }

        [Test]
        [Order(3)]
        public void SaveStoreTest()
        {
            SaveUserTest();
            var store = new Store("Store", df.ReadUser(ja.Username).Value);
            Assert.True(df.SaveStore(store).IsSuccess);
        }
        
        [Test]
        public void SaveStoreWithItemTest()
        {
            SaveUserTest();
            User user = df.ReadUser(ja.Username).Value;
            
            Store store = new Store("Store", df.ReadUser(ja.Username).Value);
            Result openRes = user.OpenStore(store);
            Assert.True(openRes.IsSuccess, $"Opening store error {openRes.Error}");
            
            Result addRes = store.AddItemToStore(new ItemInfo(10, "item1", store._storeName, "items",
                new List<string>() {"keyword1", "keyword1"}, 10.0), user);
            Assert.True(addRes.IsSuccess, $"Adding item to store error {addRes.Error}");
            
            Assert.True(df.SaveStore(store).IsSuccess, "Save store to db");
        }


        [Test]
        public void ReadUserWithStoreTest()
        {
            SaveUserWithStoreTest();
            Assert.True(df.ResetConnection().IsSuccess);
            var username = jaren.Username;
            var res = df.ReadUser(username);
            Assert.True(res.IsSuccess);
        }

        [Test]
        public void SaveUserCartTest()
        {
            
            Assert.True(df.SaveUser(ja).IsSuccess);
            Assert.True(df.SaveUser(jaren).IsSuccess);
            ja.OpenStore(alenbyStore);
            Assert.True(df.SaveStore(alenbyStore).IsSuccess);

            
            var pstation4 = new ItemInfo(100, "Playstation4", alenbyStore.GetStoreName(), "Tech",
                new List<string>(), 2500);
            var pstation5 = new ItemInfo(100, "Playstation5", alenbyStore.GetStoreName(), "Tech",
                new List<string>(), 4500);
            alenbyStore.AddItemToStore(pstation4,ja);
            alenbyStore.AddItemToStore(pstation5,ja);

            
            var resGetItem=alenbyStore.GetItem(pstation4);
            var showItem = resGetItem.Value.ShowItem();
            showItem.amount = 4;
            jaren.AddItemToCart(showItem);
            resGetItem=alenbyStore.GetItem(pstation5);
            showItem = resGetItem.Value.ShowItem();
            showItem.amount = 5;
            jaren.AddItemToCart(showItem);
            
            Assert.True(df.UpdateUser(ja).IsSuccess);
            Assert.True(df.UpdateUser(jaren).IsSuccess);
            Assert.True(df.UpdateStore(alenbyStore).IsSuccess);

        }

        [Test]
        public void ReadStoreBasketsTest()
        {
            SaveUserCartTest();
            Assert.True(df.ResetConnection().IsSuccess);
            var storeRes = df.ReadStore(alenbyStore.StoreName);
            Assert.True(storeRes.IsSuccess, storeRes.Error);
            Assert.True(storeRes.Value._basketsOfThisStore.Count == 1);
        }
        [Test]
        public void ReadUserBasketsTest()
        {
            SaveUserCartTest();
            Assert.True(df.ResetConnection().IsSuccess);
            var userRes = df.ReadUser(jaren.Username);
            Assert.True(userRes.IsSuccess, userRes.Error);
            Assert.True(userRes.Value._myCart._baskets.Count == 1);
        }
        [Test]
        public void SaveUserPurchaseTest()
        {
            SaveUserCartTest();
            var purchaseRes=jaren.BuyWholeCart(new PaymentInfo(
                userName:jaren.Username,
                idNumber:jaren.MemberInfo.Id,
                creditCardNumber:"1234567789",
                creditCardExpirationDate:"03-01-22",
                threeDigitsOnBackOfCard:"123",
                fullAddress:"TLV"
            ));
            Assert.True(df.UpdateUser(ja).IsSuccess);
            Assert.True(df.UpdateUser(jaren).IsSuccess);
            Assert.True(df.UpdateStore(alenbyStore).IsSuccess);
        }
        
        [Test]
        public void SaveUserPurchaseTest2()
        {
            Assert.True(df.SaveUser(ja).IsSuccess);
            Assert.True(df.SaveUser(jaren).IsSuccess);
            ja.OpenStore(alenbyStore);
            Assert.True(df.SaveStore(alenbyStore).IsSuccess);

            
            var pstation = new ItemInfo(100, "Playstation4", alenbyStore.GetStoreName(), "Tech",
                new List<string>(), 3500);
            var addItemRes= alenbyStore.AddItemToStore(pstation,ja);
           
            
            var resGetItem=alenbyStore.GetItem(pstation);
            var showItem = resGetItem.Value.ShowItem();
            showItem.amount = 5;
            jaren.AddItemToCart(showItem);
            
            Assert.True(df.UpdateUser(ja).IsSuccess);
            Assert.True(df.UpdateUser(jaren).IsSuccess);
            Assert.True(df.UpdateStore(alenbyStore).IsSuccess);
            Console.WriteLine($"JAREN CART HASH: {jaren._myCart.GetHashCode()}");
            var purchaseRes=jaren.BuyWholeCart(new PaymentInfo(
                userName:jaren.Username,
                idNumber:jaren.MemberInfo.Id,
                creditCardNumber:"1234567789",
                creditCardExpirationDate:"03-01-22",
                threeDigitsOnBackOfCard:"123",
                fullAddress:"TLV"
            ));
            
            Console.WriteLine($"JAREN CART HASH: {jaren._myCart.GetHashCode()}");
            // Assert.True(df.UpdateUser(ja).IsSuccess);
            Assert.True(df.UpdateUser(jaren).IsSuccess);

        }
        
        [Test]
        public void ReadUserPurchaseTest()
        {
            SaveUserPurchaseTest();
            Assert.True(df.ResetConnection().IsSuccess);
            var username = jaren.Username;
            var userRes = df.ReadUser(username);
            Assert.True(userRes.IsSuccess);
            Assert.True(userRes.Value._transHistory._purchases.Count == 1);
            var storename = alenbyStore.StoreName;
            var storeRes = df.ReadStore(storename);
            Assert.True(storeRes.IsSuccess);
            Assert.True(storeRes.Value._transactionHistory._history.Count == 1);
        }

        [Test]
        public void SaveStorePurchaseTest()
        {
            Assert.True(df.SaveUser(ja).IsSuccess);
            Assert.True(df.SaveUser(jaren).IsSuccess);
            Assert.True(df.SaveUser(dillon).IsSuccess);
            Assert.True(df.SaveStore(alenbyStore).IsSuccess);
            
            ja.OpenStore(alenbyStore);
            Assert.True(df.UpdateUser(ja).IsSuccess);
            var pstation4 = new ItemInfo(100, "Playstation4", alenbyStore.GetStoreName(), "Gaming",
                new List<string>(), 2500);
            var pstation5 = new ItemInfo(100, "Playstation5", alenbyStore.GetStoreName(), "Gaming",
                new List<string>(), 4000);
            alenbyStore.AddItemToStore(pstation4,ja);
            alenbyStore.AddItemToStore(pstation5,ja);
            ja.AppointUserToOwner(alenbyStore, jaren);
            jaren.AppointUserToManager(alenbyStore, dillon);
            
            Assert.True(df.UpdateStore(alenbyStore).IsSuccess);

        }

        [Test]
        public void ReadStorePurchaseTest()
        {
            SaveStorePurchaseTest();
            Assert.True(df.ResetConnection().IsSuccess);
            var storeName = alenbyStore.StoreName;
            alenbyStore = null;
            var res = df.ReadStore(storeName);
            Assert.True(res.IsSuccess, res.Error);
        }

        [Test]
        public void TheReadStoreTest()
        {
            SaveUserWithStoreTest();
            Assert.True(df.ResetConnection().IsSuccess);
            
            var storeRes = df.ReadStore(store3.StoreName);
            Assert.True(storeRes.IsSuccess,storeRes.Error);
            Assert.True(storeRes.Value._ownersAppointments.Count == 2);
            
            storeRes = df.ReadStore(store1.StoreName);
            Assert.True(storeRes.IsSuccess,storeRes.Error);
            Assert.True(storeRes.Value._managersAppointments.Count == 1);
            Assert.True(storeRes.Value._managersAppointments[0].User.Username == jaren.Username);
            
            var userRes = df.ReadUser(jaren.Username);
            Assert.True(userRes.IsSuccess,userRes.Error);
            Assert.True(userRes.Value.StoresOwned.Count == 2);
            Assert.True(userRes.Value.StoresOwned.ContainsKey(store3.StoreName));
            Assert.True(userRes.Value.StoresOwned.ContainsKey(store4.StoreName));
            Assert.True(userRes.Value.StoresManaged.Count == 2);
            Assert.True(userRes.Value.StoresManaged.ContainsKey(store1.StoreName));
            Assert.True(userRes.Value.StoresManaged.ContainsKey(store2.StoreName));
            
            userRes = df.ReadUser(ja.Username);
            Assert.True(userRes.IsSuccess,userRes.Error);
            Assert.True(userRes.Value.AppointedOwners.Count == 2);
            Assert.True(userRes.Value.AppointedOwners.ContainsKey(store3.StoreName));
            Assert.True(userRes.Value.AppointedOwners.KeyToValue(store3.StoreName).Count == 1);
            Assert.True(userRes.Value.AppointedOwners.ContainsKey(store4.StoreName));
            Assert.True(userRes.Value.AppointedOwners.KeyToValue(store4.StoreName).Count == 1);
        }

        [Test]
        public void NoDuplicateInstancesTest()
        {
            Assert.True(df.SaveLocalUser(ja).IsSuccess);
            // Assert.True(df.SaveUser(ja).IsSuccess);

            ja.OpenStore(store1);
            ja.OpenStore(store2);
            ja.AppointUserToManager(store1,jaren);
            ja.AppointUserToManager(store2,jaren);

            var res = df.ReadLocalUser(ja.Username);
            Assert.True(df.ResetConnection().IsSuccess);
            // var res = df.ReadUser(ja.Username);
            
            Assert.True(res.IsSuccess);
            Console.WriteLine($"Equals:\n\t{res.Value.GetHashCode()}\n\t{ja.GetHashCode()}");
            Assert.True(res.Value.GetHashCode() == ja.GetHashCode());
        }

        
        
        
        [Test]
        public void ReadStoreManagersTest()
        {
            Assert.Warn("Not Implemented");
        }
        [Test]
        public void SaveStoreWithoutGuestCartTest()
        {
            Assert.Warn("Not Implemented");
        }
        
        [Test]
        public void SaveStoreWithoutGuestHistoryTest()
        {
            Assert.Warn("Not Implemented");
        }
    }
}