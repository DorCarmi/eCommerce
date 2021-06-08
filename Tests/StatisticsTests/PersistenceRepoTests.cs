using System;
using System.Collections.Generic;
using eCommerce.Common;
using eCommerce.Statistics;
using eCommerce.Statistics.Repositories;
using NUnit.Framework;

namespace Tests.StatisticsTests
{
    [TestFixture]
    public class PersistenceRepoTests
    {

        private PersistenceStatsRepo _repo;
        public PersistenceRepoTests()
        {
            _repo = new PersistenceStatsRepo();
        }
        
        [Test]
        [Order(1)]
        public void AddLoginsTest()
        {
            Assert.True(_repo.AddLoginStat(new LoginStat(DateTime.Now.Date, "_Guest1", "guest")).IsSuccess);
            Assert.True(_repo.AddLoginStat(new LoginStat(DateTime.Now.Date, "User1", "owner")).IsSuccess);
            Assert.True(_repo.AddLoginStat(new LoginStat(DateTime.Now.AddDays(1).Date, "User2", "owner")).IsSuccess);
        }
        
        [Test]
        [Order(2)]
        public void GetLoginOfTodayTest()
        {
            Result<List<LoginStat>> loginStatsRes = _repo.GetAllLoginStatsFrom(DateTime.Now);
            Assert.True(loginStatsRes.IsSuccess);

            foreach (var stat in loginStatsRes.Value)
            {
                switch (stat.Username)
                {
                    case "_Guest1":
                        break;
                    case "User1":
                        break;
                    default:
                        Assert.Fail("Item doesnt need to exist");
                        break;
                }
            }

        }
    }
}