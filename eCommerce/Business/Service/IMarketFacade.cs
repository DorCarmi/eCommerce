using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Business.Service
{
    public interface IMarketFacade
    {
        /// <summary>
        /// Connect a new get to the system
        /// </summary>
        /// <returns>New auth token</returns>
        public Result<string> Connect();

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
        public Result Register(string username, string password);
        
        /// <summary>
        /// Log in to the system
        /// </summary>
        /// <param name="username">The user name</param>
        /// <param name="password">The user password</param>
        /// <returns>Authorization token</returns>
        // TODO add role as parameter
        public Result<string> Login(string username, string password);

        /// <summary>
        /// Logout a user form the system.
        /// </summary>
        /// <param name="token">Authorization token</param>
        public void Logout(string token);
        
        // ========== Store ========== //
        
        // TODO requirement 2.5, 2.6
        
        /// <summary>
        /// Search for product
        /// </summary>
        /// <param name="query">The product query the search</param>
        /// <param name="token">Authorization token</param>
        /// <returns>List of match products</returns>
        public Result<IEnumerable<ItemDto>> SearchForProduct(string token, string query);
        
        // ========== User ========== //
        
        // TODO purchase option

        /// <summary>
        /// Adding Item to user cart
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="itemId">The item id</param>
        /// <param name="storeId">The store id</param>
        /// <param name="amount">The amount of the item</param>
        /// <returns>Result of the request</returns>
        public Result AddItemToCart(string token, string itemId, string storeId, int amount);
        
        /// <summary>
        /// Change the amount of the item in the cart
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="itemId">The item id</param>
        /// <param name="storeId">The store id</param>
        /// <param name="amount">The amount of the item</param>
        /// <returns>Result of the request</returns>
        public Result EditItemAmountOfCart(string token, string itemId, string storeId, int amount);

        /// <summary>
        /// Get the cart of the user
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>The user cart</returns>
        public Result<CartDto> GetCart(string token);

        /// <summary>
        /// Return the total price of the cart(after discounts)
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>The total price of the cart</returns>
        public Result<int> GetPurchaseCartPrice(string token);
        
        /// <summary>
        /// Purchase the user cart
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>The result the purchase</returns>
        public Result PurchaseCart(string token);
    }
}