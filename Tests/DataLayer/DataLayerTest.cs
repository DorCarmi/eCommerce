using System;
using eCommerce.Business;
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
        private User _ja;
        private Store store1;
        private Store store2;
       
        public DataLayerTest()
        {
            df = new DataFacade();
            var info = new MemberInfo("Ja Morant", "ja@mail.com", "Ja", DateTime.Now, "Memphis");
            info.Id = "12";
            
            store1= new Store("BasketBall stuff.. buy here",_ja);
            store2= new Store("More(!) BasketBall stuff.. buy here",_ja);
            _ja = new User(Member.State, info);
            
            df.ClearTables();
        }

        [TearDown]
        public void TearDown()
        {
            df.ClearTables();
        }

        [Test]
        [Order(1)]
        public void SaveUserTest()
        {
            Assert.True(df.SaveUser(_ja).IsSuccess);
        }
        
        [Test]
        [Order(2)]
        public void ReadUserTest()
        {
            SaveUserTest();
            Assert.True(df.ReadUser(_ja.Username).IsSuccess);
        }

        [Test]
        [Order(3)]
        public void SaveStoreTest()
        {
            SaveUserTest();
            var store = new Store("Store", _ja);
            //Assert.True(df.SaveUser(founder).IsSuccess);
            _ja.OpenStore(store);
            Assert.True(df.SaveStore(store).IsSuccess);
        }


        [Test]
        public void aTest()
        {
            _ja.OpenStore(store1);
            _ja.OpenStore(store2);
            Assert.True(df.SaveUser(_ja).IsSuccess);
        }

        [Test]
        public void bTest()
        {
            var username = "Ja Morant";
            Assert.True(df.DoSomething(username).IsSuccess);
        }


    }
}