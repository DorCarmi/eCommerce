using Microsoft.EntityFrameworkCore;

namespace eCommerce.Auth.DAL
{
    public class UserContext : DbContext
    {
        private string _connectionString;

        public UserContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(_connectionString);
        
        // DB sets
        public DbSet<User> User { get; set; }
    }
}