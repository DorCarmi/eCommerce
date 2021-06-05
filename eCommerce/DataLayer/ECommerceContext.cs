using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eCommerce.Business;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.DataLayer
{
    public class ECommerceContext: DbContext
    {
        public DbSet<MemberInfo> MemberInfos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OwnerAppointment> OwnerAppointments { get; set; }
        // public DbSet<ManagerAppointment> ManagerAppointments { get; set; }
        public DbSet<Pair<Store,OwnerAppointment>> OwnedStores { get; set; }
        public DbSet<ListPair<Classroom,Course>> ListPairs { get; set; }

        public DbSet<Store> Stores { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemsInventory> ItemsInventories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //@TODO::sharon - enable test mode for db - use different db + add teardown function to context 
            optionsBuilder.EnableSensitiveDataLogging().UseSqlServer(AppConfig.GetInstance().GetData("DBConnectionString"));
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ListPair<Classroom,Course>>()
                .HasKey(p => new {p.HolderId, p.KeyId});
            builder.Entity<Pair<Store,OwnerAppointment>>()
                .HasKey(p => new {p.HolderId, p.KeyId});
            builder.Entity<OwnerAppointment>()
                .HasKey(o => new {o.Username, o.Storename});
            
            builder.Entity<Item>()
                .HasKey(p => new {p._name, p.StoreId});
        }
    }

    public class ListPair<K,V>
    {
        public string KeyId { get; set; }
        public K Key { get; set; }
        public virtual List<V> ValList { get; set; }
        public string HolderId { get; set; }
    }
    
    public class Pair<K,V>
    {
        public string KeyId { get; set; }
        public K Key { get; set; }
        // public int ValueId { get; set; }
        public V Value { get; set; }
        public string HolderId { get; set; }
    }

}