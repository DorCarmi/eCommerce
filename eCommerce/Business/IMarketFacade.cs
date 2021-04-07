using eCommerce.Auth;
using eCommerce.Common;

namespace eCommerce.Business
{
    public interface IMarketFacade
    {
        // Authorization functions

        /// <summary>
        /// Connect a new get to the system
        /// </summary>
        /// <returns>New auth token</returns>
        public Result<Token> Connect();

        /// <summary>
        /// Disconnect a user from the system
        /// </summary>
        /// <returns>The if the user has been disconnected</returns>
        public Result<bool> Disconnect();
        
        /// <summary>
        /// Register a new user to the system
        /// </summary>
        /// <param name="username">The user name</param>
        /// <param name="password">The user password</param>
        /// <returns>True if the user has been successfully registered</returns>
        public Result<bool> Register(string username, string password);
        
        /// <summary>
        /// Log in to the system
        /// </summary>
        /// <param name="username">The user name</param>
        /// <param name="password">The user password</param>
        /// <returns>Authorization token</returns>
        public Result<Token> Login(string username, string password);

        /// <summary>
        /// Logout a user form the system.
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>True if the user is logged in</returns>
        public Result<bool> Logout(Token token);
    }
}