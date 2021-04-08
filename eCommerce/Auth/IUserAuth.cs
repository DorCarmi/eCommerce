using eCommerce.Common;

namespace eCommerce.Auth
{
    public interface IUserAuth
    {
        /// <summary>
        /// Connect a new get to the system
        /// </summary>
        /// <returns>New auth token</returns>
        public Result<Token> Connect();
        
        /// <summary>
        /// Register a new user to the system as a member.
        /// </summary>
        /// <param name="username">The user name</param>
        /// <param name="password">The user password</param>
        /// <returns>Successful Result if the user has been successfully registered</returns>
        public Result Register(string username, string password);
        
        /// <summary>
        /// Log in to the system
        /// </summary>
        /// <param name="username">The user name</param>
        /// <param name="password">The user password</param>
        /// <param name="role">The user role</param>
        /// <returns>Authorization token</returns>
        public Result<Token> Login(string username, string password, UserRole role);
    }
}