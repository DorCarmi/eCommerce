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
        private readonly SHA256 _sha256;

        public ConcurrentRegisteredUserRepo()
        {
            _dictionary = new ConcurrentDictionary<string, User>();
            _sha256 = SHA256.Create();
        }

        /// <summary>
        /// Add a user to the repository if not already exists
        /// </summary>
        /// <param name="username">The user name</param>
        /// <param name="password">The user password</param>
        /// <returns>New User if the data is valid</returns>
        public Result<User> Add(string username, string password)
        {
            byte[] hashedPassword = _sha256.ComputeHash(Encoding.Default.GetBytes(password));

            User newUser = new User(username, hashedPassword);

            if (!_dictionary.TryAdd(username, newUser))
            {
                return Result.Fail<User>("User already exists"); 
            };

            return Result.Ok(newUser);
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
            User user;
            if (!_dictionary.TryGetValue(username, out user))
            {
                return false;
            }
            
            byte[] paramHashedPassword = _sha256.ComputeHash(Encoding.Default.GetBytes(password));
            return user.HashedPassword.SequenceEqual(paramHashedPassword);
        }
    }
}