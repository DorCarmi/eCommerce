using eCommerce.Auth;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Statistics.DAL
{
    public class StatsContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(AppConfig.GetInstance().GetData("StatsDBConnectionString"));
        }

        // DB sets
        public DbSet<LoginStat> Login { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<LoginStat>()
                .HasKey(p => new {p.DateTime, p.Username});
        }
    }
}