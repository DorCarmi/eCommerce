using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using eCommerce.Business;
using eCommerce.Common;

namespace Tests.Business
{
    /// <summary>
    /// <para>Implementation of synchronized use.</para>
    /// </summary>
    public class UserRepositoryMock : IRepository<IUser>
    {
        private IDictionary<string, IUser> _users;
        private long _id;
        
        public UserRepositoryMock()
        {
            _users = new Dictionary<string, IUser>();
            _id = 0;
        }

        public bool Add([NotNull] IUser user)
        {
            bool added = _users.TryAdd(_id.ToString(), user);
            _id++;
            return added;
        }

        public IUser GetOrNull([NotNull] string id)
        {
            IUser user = null;
            _users.TryGetValue(id, out user);
            return user;
        }

        public void Remove(string id)
        {
            _users.Remove(id);
        }
    }
}