using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using eCommerce.Common;

namespace eCommerce.Auth
{
    public class User
    {
        private string _username;
        private byte[] _hashedPassword;
        private IEnumerable<UserRole> _roles;

        public User(string username, byte[] hashedPassword)
        {
            _username = username;
            _hashedPassword = hashedPassword;
            _roles = new List<UserRole>();
        }

        /// <summary>
        /// Add a role to the user if not already exists.
        /// </summary>
        /// <Postcondition>
        /// If the user is a Admin then its also a Member
        /// </Postcondition> 
        /// <param name="role">The role to add</param>
        /// <returns>Result according to the invariants</returns>
        public Result AddRole(UserRole role)
        {
            if (role == UserRole.Admin && !AdminNeedToBeMemberConstraint())
            {
                return Result.Fail("In order to be admin, a user need to be a member");
            }

            if (_roles.Contains(role))
            {
                return Result.Fail("User already have this role");
            }

            return Result.Ok();
        }

        public bool HasRole(UserRole role)
        {
            return _roles.Contains(role);
        }
        
        // ========== Properties ========== //

        public IEnumerator<UserRole> Roles
        {
            get => _roles.GetEnumerator();
        }

        public string Username
        {
            get => _username;
        }

        public byte[] HashedPassword
        {
            get => _hashedPassword; 
        }

        private bool AdminNeedToBeMemberConstraint()
        {
            return _roles.Contains(UserRole.Member);
        }
    }
}