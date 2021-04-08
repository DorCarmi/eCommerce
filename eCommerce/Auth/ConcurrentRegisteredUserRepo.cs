using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace eCommerce.Auth
{
    internal class ConcurrentRegisteredUserRepo
    {
        private ConcurrentDictionary<string, byte[]> _dictionary;
        private readonly SHA256 _sha256;

        public ConcurrentRegisteredUserRepo()
        {
            _dictionary = new ConcurrentDictionary<string, byte[]>();
            _sha256 = SHA256.Create();
        }

        /// <summary>
        /// Add a user to the repository if not already exists
        /// </summary>
        /// <param name="username">The user name</param>
        /// <param name="password">The password</param>
        /// <returns>True if the suer have been added</returns>
        public bool Add(string username, string password)
        {
            byte[] hashedPassword = _sha256.ComputeHash(Encoding.Default.GetBytes(password));
            return _dictionary.TryAdd(username, hashedPassword);
        }
        
        /// <summary>
        /// Check if the user is in the repository and the password are
        /// a match
        /// </summary>
        /// <param name="username">The user name</param>
        /// <param name="password">The password</param>
        /// <returns>True if it is a valid user</returns>
        public bool IsRegistered(string username, string password)
        {
            byte[] userHashedPassword;
            if (!_dictionary.TryGetValue(username, out userHashedPassword))
            {
                return false;
            }
            
            byte[] paramHashedPassword = _sha256.ComputeHash(Encoding.Default.GetBytes(password));
            return userHashedPassword.SequenceEqual(paramHashedPassword);
        }
    }
}