using System;
using eCommerce.Business;
using eCommerce.DataLayer;
using NUnit.Framework;

namespace Tests.DataLayer
{
    /**
     * DB should be clear
     */
    public class DataLayerTest
    {
        private DataFacade df;
        private User _ja;
        public DataLayerTest()
        {
            df = new DataFacade();
            var info = new MemberInfo("Ja Morant", "ja@mail.com", "Ja", DateTime.Now, "Memphis");
            info.Id = "12";
            _ja = new User(info);
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
            Assert.True(df.ReadUser(_ja.Username).IsSuccess);
        }

        [Test]
        [Order(3)]
        public void SaveStoreTest()
        {
            var store = new Store("Store", df.ReadUser(_ja.Username).Value);
            Assert.True(df.SaveStore(store).IsSuccess);
        }
    }
}