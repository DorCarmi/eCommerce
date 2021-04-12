using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using eCommerce.Common;

namespace eCommerce.Auth
{
    internal class ConcurrentRegisteredUserRepo
    {
        private ConcurrentDictionary<string, User> _dictionary;

        public ConcurrentRegisteredUserRepo()
        {
            _dictionary = new ConcurrentDictionary<string, User>();
        }

        /// <summary>
        /// Add a user to the repository if not already exists
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>True if the user have been added</returns>
        public bool Add(User user)
        {
            return _dictionary.TryAdd(user.Username, user);
        }
        
        /// <summary>
        /// Get the user
        /// </summary>
        /// <param name="username">The user name</param>
        /// <returns>Return a User of exists</returns>
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