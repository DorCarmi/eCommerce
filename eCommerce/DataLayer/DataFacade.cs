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
            
            Console.WriteLine("1212Inserting a new User");
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
        
        public Result<IUser> ReadUser(string username)
        {
            IUser user = null;
            using (var db = new ECommerceContext())
            {
                try
                {
                    Console.WriteLine("fetching saved User");
                    user = db.Users
                        .Include(u => u.MemberInfo)
                        .Where(u => u.Username == username)
                        .SingleOrDefault();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return Result.Fail<IUser>("Unable to read User");
                    // add logging here
                }
            }
            return Result.Ok<IUser>(user);
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