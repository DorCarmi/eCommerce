﻿using eCommerce.Statistics.DAL;
using eCommerce.Statistics.Repositories;

namespace Tests.StatisticsTests
{

    public class StatsFactoryOfTests : StatsContextFactory
    {
        public override StatsContext Create()
        {
            return new StatsDbContextTests();
        }
    }
    
    public class PersistenceRepoExtenderTest : PersistenceStatsRepo
    {
        public PersistenceRepoExtenderTest(): base(new StatsFactoryOfTests())
        {
            
        }
        public void CleanDB()
        {
            using (var context = this._contextFactory.Create())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }
    }
}