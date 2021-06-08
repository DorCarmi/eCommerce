using eCommerce.Auth.DAL;

namespace eCommerce.Statistics.DAL
{
    public class StatsContextFactory
    {
        public StatsContext Create()
        {
            return new StatsContext();
        }
    }
}