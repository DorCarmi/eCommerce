using eCommerce.Common;

namespace eCommerce.Auth
{
    public interface IUserAuth
    {
        /// <summary>
        /// Connect a new guest to the system
        /// </summary>
        /// <returns>New auth token</returns>
        public string Connect();
        
        /// <summary>
        /// Disconnect a user from the system
        /// </summary>
        public void Disconnect(string token);
        
        /// <summary>
        /// Register a new user to the system as a member.
        /// </summary>
        /// <param name="username">The user name</param>
        /// <param name="password">The user password</param>
        /// <returns>Successful Result if the user has been successfully registered</returns>
        public Result Register(string username, string password);
        
        /// <summary>
        /// Log in to the system.
        /// </summary>
        /// <param name="username">The user name</param>
        /// <param name="password">The user password</param>
        /// <param name="role">The user role</param>
        /// <returns>Authorization token</returns>
        public Result<string> Login(string username, string password, AuthUserRole role);
        
        
        /// <summary>
        /// Logout a user form the system.
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>Result of the logout</returns>
        public Result Logout(string token);

        /// <summary>
        /// Check if a guest is connected to the system
        /// </summary>
        /// <param name="token">The guest authorization token</param>
        /// <returns>True if the guest is connected to the system</returns>
        public bool IsConnected(string token);
        
        /// <summary>
        /// Check if a user is registered to the system
        /// </summary>
        /// <param name="username">The user name</param>
        /// <returns>True if the user is registered to the system</returns>
        public bool IsRegistered(string username);

        /// <summary>
        /// Check if the user is logged in
        /// </summary>
        /// <param name="username">The user name</param>
        /// <returns>True if the user logged in</returns>
        public bool IsLoggedIn(string username);
        
        /// <summary>
        /// Check if the token is valid
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>True if the token is valid</returns>
        public bool IsValidToken(string token);
        
        /// <summary>
        /// Get the user authorization data if the token is valid and if
        /// The user is connected
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>Authorization data</returns>
        public Result<AuthData> GetDataIfConnected(string token);
        
        /// <summary>
        /// Get the user authorization data if the token is valid and if
        /// The user is  logged in
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>Authorization data</returns>
        public Result<AuthData> GetDataIfLoggedIn(string token);
        
    }
}