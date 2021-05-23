using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using eCommerce.Common;

namespace eCommerce.Auth
{
    public class InMemoryRegisteredUserRepo : IRegisteredUserRepo
    {
        private ConcurrentDictionary<string, User> _dictionary;

        public InMemoryRegisteredUserRepo()
        {
            _dictionary = new ConcurrentDictionary<string, User>();
        }
        
        public async Task<bool> Add(User user)
        {
            return _dictionary.TryAdd(user.Username, user);
        }
        
        public async Task<User> GetUserOrNull(string username)
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