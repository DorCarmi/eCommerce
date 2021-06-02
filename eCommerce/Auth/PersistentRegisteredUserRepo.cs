using System;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.Auth.DAL;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Auth
{
    public class PersistentRegisteredUserRepo : IRegisteredUserRepo
    {
        private UserContextFactory _contextFactory;
        
        public PersistentRegisteredUserRepo()
        {
            _contextFactory = new UserContextFactory();
        }

        public async Task<bool> Add(User user)
        {
            bool added = false;
            using (var context = _contextFactory.Create())
            {
                await context.User.AddAsync(user);
                added = await context.SaveChangesAsync() == 1;
            }

            return added;
        }

        public async Task<User> GetUserOrNull(string username)
        {
            User user = null;
            using (var context = _contextFactory.Create())
            {
                try
                {
                    user = await context.User.SingleAsync(user => user.Username.Equals(username));
                }
                catch (InvalidOperationException e)
                {
                    return null;
                }
            }

            return user;
        }
    }
}