using System;
using eCommerce.Business;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Auth.DAL
{
    public class UserContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(AppConfig.GetInstance().GetData("AuthDBConnectionString"));
        }

        // DB sets
        public DbSet<User> User { get; set; }
    }
}