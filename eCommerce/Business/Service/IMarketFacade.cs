using System.Collections;
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
        
        /// <summary>
        /// Add new item to the sore
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="item">The new item</param>
        /// <returns>Result of the item addition</returns>
        public Result AddNewItemToStore(string token,  ItemDto item);
        
        /// <summary>
        /// Edit the item
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="item">The new item</param>
        /// <returns>Result of the edit</returns>
        public Result EditItemAmountInStore(string token, ItemDto item);
        
        /// <summary>
        /// Remove item from store
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The sore id</param>
        /// <param name="itemId">The item id</param>
        /// <returns>Result of the product removal</returns>
        public Result RemoveProductFromStore(string token, string storeId, string itemId);
        
        // TODO requirement 4.2

        /// <summary>
        /// Appoint a user as a coOwner to the store
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <param name="appointedUserId">The appointment user</param>
        /// <returns>Result of the appointment</returns>
        public Result AppointCoOwner(string token, string storeId, string appointedUserId);
        
        /// <summary>
        /// Appoint a user as a new manager to the sore 
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <param name="appointedManagerUserId">The appointment manager</param>
        /// <returns>Result of the appointment</returns>
        public Result AppointManager(string token, string storeId, string appointedManagerUserId);
        
        // TODO how to define and send the permission
        /// <summary>
        /// Update the manager permission
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <param name="appointedManagerUserId">The User manager id</param>
        /// <param name="permissions">The updated permission</param>
        /// <returns>Result of the update</returns>
        public Result UpdateManagerPermission(string token, string storeId, string appointedManagerUserId, IList<StorePermission> permissions);

        /// <summary>
        /// Get all the staff of the store and their permissions
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The storeId</param>
        /// <returns>List of all the staff and their permissions</returns>
        public Result<IEnumerable<StaffPermission>> GetStoreStaffAndTheirPermissions(string token, string storeId);
        
        /// <summary>
        /// Return all the purchase history of a store
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The storeId</param>
        /// <returns>List of the purchase history in a store</returns>
        public Result<IEnumerable<PurchaseHistory>> GetPurchaseHistoryOfStore(string token, string storeId);

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
        
        /// <summary>
        /// Open a new store for the user.
        /// The name need to be unique
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeName">The store name</param>
        /// <returns>Result of the request</returns>
        public Result OpenStore(string token, string storeName);

        /// <summary>
        /// Get the purchase history of the user 
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>The purchase history</returns>
        public Result<IEnumerable<PurchaseHistory>> GetPurchaseHistory(string token);
        
        // ========== Admin ========== //
        
        /// <summary>
        /// Get the history purchase of a user
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <param name="ofUserId">The user id</param>
        /// <returns>The history purchase</returns>
        public Result<IEnumerable<PurchaseHistory>> AdminGetPurchaseHistoryUser(string token, string storeId, string ofUserId);
        
        /// <summary>
        /// Get the history purchase of a store
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <returns>The history purchase</returns>
        public Result<IEnumerable<PurchaseHistory>> AdminGetPurchaseHistoryStore(string token, string storeId);

    }
}