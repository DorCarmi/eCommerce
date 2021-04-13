using System.Collections;
using System.Collections.Generic;
using eCommerce.Auth;
using eCommerce.Common;

namespace eCommerce.Business.Service
{
    public interface IMarketFacade
    {
        // ========== Authorization functions ==========

        /// <summary>
        /// Connect a new get to the system
        /// </summary>
        /// <returns>New auth token</returns>
        public string Connect();

        /// <summary>
        /// Disconnect a user from the system
        /// </summary>
        public void Disconnect(string token);
        
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
        /// <param name="guestToken">The guest Authorization token</param>
        /// <param name="username">The user name</param>
        /// <param name="password">The user password</param>
        /// <returns>Authorization token</returns>
        public Result<string> Login(string guestToken ,string username, string password);

        /// <summary>
        /// Logout a user form the system.
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>New guest Authorization token</returns>
        public string Logout(string token);

        // ========== Search option ==========
        
        /**
         * TODO: When we are searching for something we can
         *      1. Return all the matches
         *      2. Use page number and page size for example
         *          page size = 20
         *          the search result have 100 options
         *          then page 1 = the 1-20 result from the list
         *          page 2 = 20-40 from list
         *          page 3 = 40-60 from list ...
         */
        
        /**
         * TODO: How to add the parameter of the filters
         *  Using a format and then parse it or few optional params like price and category ...
         */
        
        /// <summary>
        /// Search for product
        /// </summary>
        /// <param name="query">The product query the search</param>
        /// <returns>List of match products</returns>
        public Result<IEnumerable<ItemDto>> SearchForProduct(string query);
        
        // ========== Cart ==========

        // TODO: Add to role of the user?
        
        /// <summary>
        /// Add a product to user cart
        /// </summary>
        /// <param name="username">The user name</param>
        /// <param name="productId">Product id</param>
        /// <returns>True if the product has been added</returns>
        public Result<bool> AddProductToCart(string username, long productId);

    }
}