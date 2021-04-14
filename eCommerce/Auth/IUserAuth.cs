﻿using eCommerce.Common;

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
        /// Log in to the system. Delete the current guest token
        /// </summary>
        /// <param name="guestToken">The guest Authorization token</param>
        /// <param name="username">The user name</param>
        /// <param name="password">The user password</param>
        /// <param name="role">The user role</param>
        /// <returns>Authorization token</returns>
        public Result<string> Login(string guestToken ,string username, string password, AuthUserRole role);
        
        /// <summary>
        /// Try to log in to the system
        /// This method to not delete the current guest token 
        /// </summary>
        /// <param name="guestToken">The guest Authorization token</param>
        /// <param name="username">The user name</param>
        /// <param name="password">The user password</param>
        /// <param name="role">The user role</param>
        /// <returns>Authorization token</returns>
        public Result TryLogin(string guestToken ,string username, string password, AuthUserRole role);
        
        /// <summary>
        /// Logout a user form the system.
        /// The user need to be logged in
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>New guest Authorization token</returns>
        public Result<string>Logout(string token);

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
        /// Get the user authorization data if the token is valid
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>Authorization data</returns>
        public Result<AuthData> GetData(string token);
    }
}