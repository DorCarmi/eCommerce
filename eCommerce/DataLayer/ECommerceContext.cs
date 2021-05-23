using eCommerce.Business;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.DataLayer
{
    public class ECommerceContext : DbContext
    {
        public DbSet<MemberInfo> MemberInfos { get; set; }

        // The following configures EF to create a Sqlite database file as `C:\blogging.db`.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(@"Data Source=.\Persistence\eCommerce.db");
            // => options.UseSqlite(@"Data Source={AppDir}\Persistence\eCommerce.db");
    }
}