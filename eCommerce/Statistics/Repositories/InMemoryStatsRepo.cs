using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Statistics.Repositories
{
    public class InMemoryStatsRepo : StatsRepo
    {

        private List<LoginStat> _statLogins;
        
        public InMemoryStatsRepo()
        {
            _statLogins = new List<LoginStat>();
        }
        
        public Result AddLoginStat(LoginStat stat)
        {
            _statLogins.Add(stat);
            return Result.Ok();
        }

        public Result<List<LoginStat>> GetAllLoginStatsFrom(DateTime date)
        {
            List<LoginStat> loginStats = new List<LoginStat>();
            DateTime cateComponent = date.Date;
            foreach (var stat in _statLogins)
            {
                if (stat.DateTime.Date.Equals(cateComponent))
                {
                    loginStats.Add(stat);
                }
            }

            return Result.Ok(loginStats);
        }

        public Result<int> GetNumberOfLoginStatsFrom(DateTime date, string userType)
        {
            int number = 0;
            DateTime cateComponent = date.Date;
            foreach (var stat in _statLogins)
            {
                if (stat.DateTime.Date.Equals(cateComponent) && stat.UserType.Equals(userType))
                {
                    number++;
                }
            }

            return Result.Ok(number);
        }
    }
}