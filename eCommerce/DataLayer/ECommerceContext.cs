using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eCommerce.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace eCommerce.DataLayer
{
    public class ECommerceContext: DbContext
    {
        public DbSet<MemberInfo> MemberInfos { get; set; }
        public DbSet<User> Users { get; set; }
       // public DbSet<ListPair<Classroom,Course>> ListPairs { get; set; }

        public DbSet<Store> Stores { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemsInventory> ItemsInventories { get; set; }
        public DbSet<ManagerAppointment> ManagerAppointments { get; set; }
        public DbSet<OwnerAppointment> OwnerAppointments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //@TODO::sharon - enable test mode for db - use different db + add teardown function to context 
            optionsBuilder.EnableSensitiveDataLogging().UseSqlServer(AppConfig.GetInstance().GetData("DBConnectionString"));
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
           /* builder.Entity<ListPair<Classroom,Course>>()
                .HasKey(p => new {p.HolderId, p.KeyId});*/
            
            builder.Entity<Item>()
                .HasKey(p => new {p._name, p.StoreId});

            var permissionsConverter = new ValueConverter<List<StorePermission>, string>(
                v => string.Join(",", v), 
                v => ConvertStringToPermissionList(v));
            
            builder.Entity<ManagerAppointment>()
                .HasKey(p => new {p.Username, p.StoreName});
            builder.Entity<ManagerAppointment>().Property(nameof(ManagerAppointment._permissions)).HasConversion(permissionsConverter);

            builder.Entity<OwnerAppointment>()
                .HasKey(p => new {p.Username, p.StoreName});
            builder.Entity<OwnerAppointment>().Property(nameof(OwnerAppointment._permissions)).HasConversion(permissionsConverter);

        }
        
        public List<StorePermission> ConvertStringToPermissionList(string permissionsString) 
        {
            List<StorePermission> permissions = new List<StorePermission>();
            string[] vals = permissionsString.Split(new[] {';'});
            foreach (var stringPermission in vals)
            {
                permissions.Add(Enum.Parse<StorePermission>(stringPermission));
            }

            return permissions;
        }
        
    }
    
    public class ListPair<K,V>
    {
        public int KeyId { get; set; }
        public K Key { get; set; }
        public virtual List<V> ValList { get; set; }
        public int HolderId { get; set; }
    }
    
    public class Pair<K,V>
    {
        public int ClassroomId { get; set; }
        public K Classroom { get; set; }
        public int CourseId { get; set; }
        public V Course { get; set; }
        public int StudentId { get; set; }
    }
}