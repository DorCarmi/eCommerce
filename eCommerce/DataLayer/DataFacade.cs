﻿using System;
using System.Linq;
using eCommerce.Business;
using eCommerce.Common;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.DataLayer
{
    public class DataFacade
    {
        private ECommerceContext db;

        public void init()
        {
            try
            {
                db = new ECommerceContext();
                Console.WriteLine("INIT DF!!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //log uable to connect to sql server
            }
        }

        
        public Result SaveUser(User user)
        {
            // synchronize all fields which arent compatible with EF (all dictionary properties)
            user.SyncFromBusiness();

            Console.WriteLine("Inserting a new User");
            try
            {
                // each user only saves the stores he founded
                
                // foreach (Pair<Store,ManagerAppointment> pair in user.storesManagedBackup)
                // {
                //     db.Entry(pair.Key).State = EntityState.Unchanged;
                //     db.Entry(pair.Value).State = EntityState.Unchanged;
                // }
                //
                // foreach (Pair<Store, OwnerAppointment> pair in user.storesOwnedBackup)
                // {
                //     if (!user.StoresFounded.ContainsKey(pair.Key))
                //     {
                //         db.Entry(pair.Key).State = EntityState.Unchanged;
                //         db.Entry(pair.Value).State = EntityState.Unchanged;
                //     }
                // }
                //
                // foreach (var pair in user.appointedOwnersBackup)
                // {
                //     if (!user.StoresFounded.ContainsKey(pair.Key))
                //     {
                //         db.Entry(pair.Key).State = EntityState.Unchanged;
                //     }
                //     foreach (var owner in pair.ValList)
                //     {
                //         db.Entry(owner.User).State = EntityState.Unchanged;
                //     }
                // }
                //
                // foreach (var pair in user.appointedManagersBackup)
                // {
                //     if (!user.StoresFounded.ContainsKey(pair.Key))
                //     {
                //         db.Entry(pair.Key).State = EntityState.Unchanged;
                //     }
                //     
                //     foreach (var manager in pair.ValList)
                //     {
                //         db.Entry(manager.User).State = EntityState.Unchanged;
                //     }
                // }
                
                db.Add(user);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);   
                return Result.Fail("Unable to Save User");
                // add logging here
            }

        return Result.Ok();
        }
        
        public Result<User> ReadUser(string username)
        {
            User user = null;
            
            try
            {
                Console.WriteLine("fetching saved User");
                user = db.Users
                    .Include(u => u.MemberInfo)
                    .Include(u => u.storesFoundedBackup)
                    // include inner Owned-Stores entities
                    .Include(u=> u.storesOwnedBackup)
                    .ThenInclude(p => p.Key)
                    .Include(u=> u.storesOwnedBackup)
                    .ThenInclude(p => p.Value)
                    .ThenInclude(o => o.User)
                    // include inner Managed-Stores entities
                    .Include(u=> u.storesManagedBackup)
                    .ThenInclude(p => p.Key)
                    .Include(u=> u.storesManagedBackup)
                    .ThenInclude(p => p.Value)
                    .ThenInclude(m => m.User)
                    // include inner Appointed-Owners entities
                    .Include(u=> u.appointedOwnersBackup)
                    .ThenInclude(p => p.Key)
                    .Include(u=> u.appointedOwnersBackup)
                    .ThenInclude(p => p.ValList)
                    .ThenInclude(o => o.User)
                    // include inner Appointed-Managers entities
                    .Include(u=> u.appointedManagersBackup)
                    .ThenInclude(p => p.Key)
                    .Include(u=> u.appointedManagersBackup)
                    .ThenInclude(p => p.ValList)
                    .ThenInclude(m => m.User)

                    .SingleOrDefault(u => u.Username == username);
                
            }
            catch (Exception e)
                {
                    Console.WriteLine(e);
                    return Result.Fail<User>("Unable to read User");
                    // add logging here
                }
            
            if (user == null)
            {
                return Result.Fail<User>($"No user called {username}");
            }
            // synchronize all fields which arent compatible with EF (all dictionary properties) 
            user.SyncToBusiness();

            return Result.Ok<User>(user);
        }

        public Result UpdateUser(User user)
        {
            try
            {
                Console.WriteLine("updating saved User");
                user.SyncFromBusiness();
                
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Result.Fail<User>("Unable to read User");
                // add logging here
            }
            return Result.Ok();
        }

        public Result SaveStore(Store store)
        { 
            try
            { 
                db.Add(store);
                db.Entry(store._founder).State = EntityState.Unchanged;
                db.Entry(store._founder.MemberInfo).State = EntityState.Unchanged;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);   
                return Result.Fail("Unable to Save Store");
                // add logging here
            }
            return Result.Ok();
        }
        
        
        public Result DoSomething(string username)
        {
            User user1 = null;
            
            User user2 = null;

            using (var db = new ECommerceContext())
            {
                try
                {
                    Console.WriteLine("fetching saved User1");
                    user1 = db.Users
                        .Include(u => u.MemberInfo)
                        .Where(u => u.Username == username)
                        .SingleOrDefault();
                    Console.WriteLine("fetching saved User2");
                    user2 = db.Users
                        .Include(u => u.MemberInfo)
                        .Where(u => u.Username == username)
                        .SingleOrDefault();
                    if(user1 != user2)
                        return Result.Fail("user instances are not equal");
                    Console.WriteLine("are equal? {0}",user1 == user2);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return Result.Fail("Unable to read User");
                    // add logging here
                }
                
            }
            return Result.Ok();
        }

        public Result ClearTables()
        {
            try
            {
                db = new ECommerceContext();
                {
                   Console.WriteLine("clearing DB!");
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Result.Fail("Unable to read User");
                // add logging here
            }

            return Result.Ok();
        }
    }
}