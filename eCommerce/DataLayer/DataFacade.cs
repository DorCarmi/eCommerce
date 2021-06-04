using System;
using System.Linq;
using eCommerce.Business;
using eCommerce.Common;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.DataLayer
{
    public class DataFacade
    {

        public Result SaveUser(User user)
        {
            
            Console.WriteLine("Inserting a new User");
            using (var db = new ECommerceContext())
            {
                try
                {
                    Console.WriteLine("Inserting a new User");
                    db.Add(user);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);   
                    return Result.Fail("Unable to Save User");
                    // add logging here
                }

            }
            return Result.Ok();
        }
        
        public Result<User> ReadUser(string username)
        {
            User user = null;
            using (var db = new ECommerceContext())
            {
                try
                {
                    Console.WriteLine("fetching saved User");
                    user = db.Users
                        .Include(u => u.MemberInfo)
                        .SingleOrDefault(u => u.Username == username);

                    if (user == null)
                    {
                        return Result.Fail<User>($"No user called {username}");
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return Result.Fail<User>("Unable to read User");
                    // add logging here
                }
            }
            return Result.Ok<User>(user);
        }
        
        public Result SaveStore(Store store)
        {
            using (var db = new ECommerceContext())
            {
                try
                { 
                    db.Add(store);
                    db.Entry(store._founder).State = EntityState.Unchanged;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.InnerException);   
                    return Result.Fail("Unable to Save Store");
                    // add logging here
                }

            }
            return Result.Ok();
        }
        
        public Result<IUser> DoSomething(string username)
        {
            IUser user1 = null;
            
            IUser user2 = null;

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

                    Console.WriteLine("are equal? {0}",user1 == user2);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return Result.Fail<IUser>("Unable to read User");
                    // add logging here
                }
                
            }
            return Result.Fail<IUser>("bad.. very very bad..");
        }
    }
}