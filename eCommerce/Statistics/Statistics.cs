using System;
using System.Collections.Generic;
using eCommerce.Common;
using eCommerce.Statistics.Repositories;

namespace eCommerce.Statistics
{
    public class Statistics : IStatisticsService
    {
        private static Statistics _instance = new Statistics(new InMemoryStatsRepo());
        private StatsRepo _statsRepo;

        private Statistics(StatsRepo statsRepo)
        {
            _statsRepo = statsRepo;
        }

        public static Statistics GetInstance()
        {
            return _instance;
        }
        
        public static Statistics GetInstanceForTests(StatsRepo statsRepo)
        {
            return new Statistics(statsRepo);
        }

        public Result AddLoggedIn(DateTime dateTime, string username, string userType)
        {
            dateTime = dateTime.Date;
            return _statsRepo.AddLoginStat(new LoginStat(dateTime, username, userType));
        }

        public Result<LoginDateStat> GetLoginStatsOn(DateTime date)
        {
            Result<List<LoginStat>> statsRes = _statsRepo.GetAllLoginStatsFrom(date);
            if (statsRes.IsFailure)
            {
                return Result.Fail<LoginDateStat>(statsRes.Error);
            }

            Dictionary<string, int> loginPerType = new Dictionary<string, int>();
            foreach (var loginStat in statsRes.Value)
            {
                if (!loginPerType.ContainsKey(loginStat.UserType))
                {
                    loginPerType[loginStat.UserType] = 1;
                    continue;
                }
                loginPerType[loginStat.UserType] = loginPerType[loginStat.UserType] + 1;
            }

            List<Tuple<string, int>> loginStats = new List<Tuple<string, int>>();
            foreach (var key in loginPerType.Keys)
            {
                loginStats.Add(new Tuple<string, int>(key, loginPerType[key]));
            }

            return Result.Ok(new LoginDateStat(loginStats));
        }
    }
}