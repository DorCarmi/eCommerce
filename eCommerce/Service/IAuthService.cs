﻿using eCommerce.Business;
using eCommerce.Common;

namespace eCommerce.Service
{
    public interface IAuthService
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
        ///<Test>
        ///TestRegisterToSystemSuccess
        ///TestRegisterToSystemFailure
        ///</Test>
        /// <param name="token">The Authorization token</param>
        /// <param name="memberInfoDto">The user information</param>
        /// <param name="password">The user password</param>
        /// <returns>Successful Result if the user has been successfully registered</returns>
        public Result Register(string token, MemberInfo memberInfo, string password);
        
        /// <summary>
        /// Log in to the system
        /// </summary>
        /// <Test>
        ///TestLoginSuccess
        /// TestLoginFailure
        /// </Test>
        /// <param name="guestToken">The guest Authorization token</param>
        /// <param name="username">The user name</param>
        /// <param name="password">The user password</param>
        /// <param name="role">The user role</param>
        /// <returns>Authorization token</returns>
        public Result<string> Login(string guestToken ,string username, string password, ServiceUserRole role);
        
        /// <summary>
        /// Logout a user form the system.
        /// </summary>
        /// <Test>
        ///TestLogoutSuccess
        ///TestLogoutFailure
        /// </Test>
        /// <param name="token">Authorization token</param>
        /// <returns>New guest Authorization token</returns>
        public Result<string> Logout(string token);
    }
}