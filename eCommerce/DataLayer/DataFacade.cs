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
    }
}