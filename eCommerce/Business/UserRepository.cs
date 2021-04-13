using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;

namespace eCommerce.Business
{
    public class UserRepository
    {

        private IDictionary<string, IUser> _users;
        
        public UserRepository()
        {
            _users = new ConcurrentDictionary<string, IUser>();
        }

        public bool Add([NotNull] IUser user)
        {
            return _users.TryAdd(user.UserId, user);
        }
    }
}