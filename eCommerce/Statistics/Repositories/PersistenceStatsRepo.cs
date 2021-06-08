using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Common;
using eCommerce.Statistics.DAL;

namespace eCommerce.Statistics.Repositories
{
    public class PersistenceStatsRepo : StatsRepo
    {
        private StatsContextFactory _contextFactory;

        public PersistenceStatsRepo()
        {
            _contextFactory = new StatsContextFactory();
        }

        public Result AddLoginStat(LoginStat stat)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    context.Add(stat);
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    return Result.Fail("Error saving data");
                }
            }

            return Result.Ok();
        }

        public Result<List<LoginStat>> GetAllLoginStatsFrom(DateTime date)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    return Result.Ok(context.Login.Where(ls => ls.DateTime.Equals(date.Date)).ToList());
                }
                catch (Exception e)
                {
                    return Result.Fail<List<LoginStat>>("Error saving data");
                }
            }
        }
    }
}