using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class UserRepository : IRepository<IUser>
    {
        private IDictionary<string, IUser> _users;
        
        // TODO change it when we will use DB
        private ConcurrentIdGenerator _idGenerator;
        
        public UserRepository()
        {
            _users = new ConcurrentDictionary<string, IUser>();
            _idGenerator = new ConcurrentIdGenerator(0);
        }

        public bool Add([NotNull] IUser user)
        {
            bool added = _users.TryAdd(user.UserId, user);
            if (added)
            {
                user.UserId = GetAndIncrementID().ToString();
            }

            return added;
        }

        public IUser GetOrNull([NotNull] string username)
        {
            IUser user = null;
            _users.TryGetValue(username, out user);
            return user;
        }

        private long GetAndIncrementID()
        {
            return _idGenerator.MoveNext();
        }
    }
}