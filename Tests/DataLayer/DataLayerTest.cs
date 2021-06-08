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
        private Store store1;
        private Store store2;
       
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

            ja = new User(info1);
            jaren = new User(info2);
            store1= new Store("BasketBall stuff.. buy here",ja);
            store2= new Store("More(!) BasketBall stuff.. buy here",ja);

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
            Assert.True(df.ReadUser(ja.Username).IsSuccess);
        }

        [Test]
        public void SaveUserWithStoreTest_Fail()
        {
            // incorrect order of operations. correct order is
            // create user > save user
            // save founder & save store > founder.open(Store)
            // save co-owner > owner.appoint(co-owner)  ...  (same for manager)
            // update user at any time (after save)
            
            var store3= new Store("EVEN MORE!! BasketBall stuff.. buy here",ja);
            var store4= new Store("ok we had too much BasketBall stuff.. buy here",ja);
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
            Assert.True(df.SaveUser(ja).IsSuccess);
            Assert.True(df.SaveUser(jaren).IsFailure);
            
        } [Test]
        public void SaveUserWithStoreTest_Success()
        {
            Assert.True(df.SaveUser(ja).IsSuccess);
            var store3= new Store("EVEN MORE!! BasketBall stuff.. buy here",ja);
            var store4= new Store("ok we had too much BasketBall stuff.. buy here",ja);
            ja.OpenStore(store1);
            ja.OpenStore(store2);
            ja.OpenStore(store3);
            ja.OpenStore(store4);
            Assert.True(df.SaveUser(jaren).IsSuccess);
            ja.AppointUserToManager(store1,jaren);
            ja.AppointUserToManager(store2,jaren);
            ja.AppointUserToOwner(store3, jaren);
            ja.AppointUserToOwner(store4, jaren);
            var prems = new List<StorePermission>() {StorePermission.ChangeItemPrice, StorePermission.ControlStaffPermission, StorePermission.EditStorePolicy, StorePermission.AddItemToStore};
            ja.UpdatePermissionsToManager(store2,jaren, prems);
            Assert.True(df.UpdateUser(ja).IsSuccess);
            Assert.True(df.UpdateUser(jaren).IsSuccess);
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
            SaveUserWithStoreTest_Success();
            var username = ja.Username;
            var res = df.ReadUser(username);
            Assert.True(res.IsSuccess);
        }
    }
}