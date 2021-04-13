using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using eCommerce.Common;

namespace eCommerce.Auth
{
    internal class ConcurrentRegisteredUserRepo : IRegisteredUserRepo
    {
        private ConcurrentDictionary<string, User> _dictionary;

        public ConcurrentRegisteredUserRepo()
        {
            _dictionary = new ConcurrentDictionary<string, User>();
        }
        
        public bool Add(User user)
        {
            return _dictionary.TryAdd(user.Username, user);
        }
        
        public User GetUserOrNull(string username)
        {
            User user;
            if (!_dictionary.TryGetValue(username, out user))
            {
                user = null;
            }
            return user;
        }
    }
}