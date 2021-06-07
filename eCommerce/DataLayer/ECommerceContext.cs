﻿using System.Collections.Generic;
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
        public DbSet<ManagerAppointment> ManagerAppointments { get; set; }
        public DbSet<Pair<Store,OwnerAppointment>> OwnedStores { get; set; }
        public DbSet<Pair<Store,ManagerAppointment>> ManagedStores { get; set; }
        public DbSet<ListPair<Store,OwnerAppointment>> AppointedOwners { get; set; }
        public DbSet<ListPair<Store,ManagerAppointment>> AppointedManagers { get; set; }

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
            //set composite Primary-Key for entities of type OwnerAppointment
            builder.Entity<OwnerAppointment>()
                .HasKey(o => new {o.Ownername, o.OwnedStorename});
            
            //set composite Primary-Key for entities of type ManagerAppointment
            builder.Entity<ManagerAppointment>()
                .HasKey(o => new {o.Managername, o.ManagedStorename});
            
            //set composite Primary-Key for every entity of type Pair<Store,OwnerAppointment>
            builder.Entity<Pair<Store,OwnerAppointment>>()
                .HasKey(p => new {p.HolderId, p.KeyId});
            
            //set composite Primary-Key for every entity of type Pair<Store,ManagerAppointment>
            builder.Entity<Pair<Store,ManagerAppointment>>()
                .HasKey(p => new {p.HolderId, p.KeyId});
            
            //set composite Primary-Key for every entity of type Pair<Store,OwnerAppointment>
            builder.Entity<ListPair<Store,OwnerAppointment>>()
                .HasKey(p => new {p.HolderId, p.KeyId});
            
            //set composite Primary-Key for every entity of type Pair<Store,ManagerAppointment>
            builder.Entity<ListPair<Store,ManagerAppointment>>()
                .HasKey(p => new {p.HolderId, p.KeyId});
            
            
            //set composite Primary-Key for entities of type Item
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
        public V Value { get; set; }
        public string HolderId { get; set; }
    }
}