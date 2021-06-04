using System;
using eCommerce.Business;
using eCommerce.DataLayer;
using NUnit.Framework;

namespace Tests.DataLayer
{
    public class DataLayerTest
    {
        private DataFacade df;
        
        [SetUp]
        public void Setup()
        {
            df = new DataFacade();
            //clear database
        }

        [Test]
        public void SaveUserTest()
        {
            var info = new MemberInfo("Ja Morant", "ja@mail.com", "Ja", DateTime.Now, "Memphis");
            info.Id = "12";
            var  ja = new User(info);
            Assert.True(df.SaveUser(ja).IsSuccess);
        }
        
        [Test]
        public void ReadUserTest()
        {
            var username = "Ja Morant";
            Assert.True(df.ReadUser(username).IsSuccess);
        }
        
        
        [Test]
        public void aTest()
        {
            var username = "Ja Morant";
            Assert.True(df.DoSomething(username).IsSuccess);
        }

        [Test]
        public void MemberInfoDateTimeTest()
        {
            
        }

    }
}