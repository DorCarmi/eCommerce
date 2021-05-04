using System;
using System.Collections.Generic;
using eCommerce.Business.Service;
using eCommerce.Common;
using eCommerce.Service;

namespace eCommerce.Business
{
    public interface IMarketFacade
    {
        #region UserManage
        
        // ========== Connection and Authorization ========== //
        
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
        public Result<string> Login(string guestToken ,string username, string password, UserToSystemState role);
        
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
        
        bool IsUserConnected(string token);

        /// <summary>
        /// Get the purchase history of the user 
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>The purchase history</returns>
        public Result<IList<PurchaseRecord>> GetPurchaseHistory(string token);
        
        /// <summary>
        /// Appoint a user as a coOwner to the store
        /// </summary>
        /// <Test>
        /// TestAppointCoOwnerSuccess
        /// TestAppointCoOwnerFailureInvalid
        /// TestAppointCoOwnerFailureLogic
        /// </Test>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <param name="appointedUserId">The appointment user</param>
        /// <returns>Result of the appointment</returns>
        public Result AppointCoOwner(string token, string storeId, string appointedUserId);
        
        /// <summary>
        /// Appoint a user as a new manager to the sore 
        /// </summary>
        /// <Test>
        /// TestAppointManagerSuccess
        /// TestAppointManagerFailureInvalid
        /// TestAppointManagerFailureLogic
        /// </Test>
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
        /// <param name="managersUserId">The user id of the manger</param>
        /// <param name="permissions">The updated permission</param>
        /// <returns>Result of the update</returns>
        public Result UpdateManagerPermission(string token, string storeId, string managersUserId, IList<StorePermission> permissions);
        
        // TODO how to define and send the permission
        /// <summary>
        /// Remove the manager permission
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <param name="managersUserId">The user id of the manger</param>
        /// <param name="permissions">The updated permission</param>
        /// <returns>Result of the remove</returns>
        public Result RemoveManagerPermission(string token, string storeId, string managersUserId,
            IList<StorePermission> permissions);


        /// <summary>
        /// Get all the staff of the store and their permissions
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The storeId</param>
        /// <returns>List of all the staff and their permissions</returns>
        public Result<IList<Tuple<string, IList<StorePermission>>>> GetStoreStaffAndTheirPermissions(string token,
            string storeId);

        /// <summary>
        /// Get the history purchase of a user
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <param name="ofUserId">The user id</param>
        /// <returns>The history purchase</returns>
        public Result<IList<PurchaseRecord>> AdminGetPurchaseHistoryUser(string token, string ofUserId);
        
        /// <summary>
        /// Get the history purchase of a store
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <returns>The history purchase</returns>
        public Result<IList<PurchaseRecord>> AdminGetPurchaseHistoryStore(string token, string storeID);
        
        
        #endregion
        
        #region ItemsAndStores

        /// <summary>
        /// Search for item
        /// </summary>
        /// <param name="query">The item query the search</param>
        /// <param name="token">Authorization token</param>
        /// <returns>List of match products</returns>
        public Result<IEnumerable<IItem>> SearchForItem(string token, string query);
        
        /// <summary>
        /// Search for item by price range
        /// </summary>
        /// <param name="query">The item query the search</param>
        /// <param name="token">Authorization token</param>
        /// <param name="from">From price</param>
        /// <param name="to">To price</param>
        /// <returns>List of match products</returns>
        public Result<IEnumerable<IItem>> SearchForItemByPriceRange(string token, string query, double from = 0.0, double to = Double.MaxValue);
        
        /// <summary>
        /// Search for item by category
        /// </summary>
        /// <param name="query">The item query the search</param>
        /// <param name="token">Authorization token</param>
        /// <param name="category">Search category</param>
        /// <returns>List of match products</returns>
        public Result<IEnumerable<IItem>> SearchForItemByCategory(string token, string query, string category);

        /// <summary>
        /// Search for store
        /// </summary>
        /// <param name="query">The store query the search</param>
        /// <param name="token">Authorization token</param>
        /// <returns>List of match stores</returns>
        public Result<IEnumerable<string>> SearchForStore(string token, string query);
        
        /// <summary>
        /// Get all the store information
        /// </summary>
        /// <param name="token">The Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <returns>The store information</returns>
        public Result<IStore> GetStore(string token, string storeId);
        
        /// <summary>
        /// Get all the store items
        /// </summary>
        /// <param name="token">The Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <returns>The store items</returns>
        public Result<IEnumerable<IItem>> GetAllStoreItems(string token, string storeId);
        
        /// <summary>
        /// Get the info of an item
        /// </summary>
        /// <param name="token">The Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <param name="itemId">The item id</param>
        /// <returns>The item information</returns>
        public Result<IItem> GetItem(string token, string storeId, string itemId);
        
        /// <summary>
        /// Get all the store ids of the user owns
        /// </summary>
        /// <param name="token">The Authorization token</param>
        /// <returns>All the owned store ids</returns>
        public Result<List<string>> GetStoreIds(string token);
        
        #endregion
        
        // ========== Store ========== //
        
        #region UserBuyingFromStores
        
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
        public Result<ICart> GetCart(string token);

        /// <summary>
        /// Return the total price of the cart(after discounts)
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>The total price of the cart</returns>
        public Result<double> GetPurchaseCartPrice(string token);
        
        /// <summary>
        /// Purchase the user cart
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>The result the purchase</returns>
        public Result PurchaseCart(string token, PaymentInfo paymentInfo);
        
        #endregion

        #region StoreManage
        
        /// <summary>
        /// Open a new store for the user.
        /// The name need to be unique
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeName">The store name</param>
        /// <param name="item">The start product of a sotre</param>
        /// <returns>Result of the request</returns>
        public Result OpenStore(string token, string storeName, IItem item);
        
        /// <summary>
        /// Add new item to the sore
        /// </summary>
        /// <Test>
        /// TestAddNewItemToStoreSuccess
        /// TestAddNewItemToStoreFailureInput
        /// TestAddNewItemToStoreFailureAccess
        /// </Test>
        /// <param name="token">Authorization token</param>
        /// <param name="item">The new item</param>
        /// <returns>Result of the item addition</returns>
        public Result AddNewItemToStore(string token,  IItem item);
        
        /// <summary>
        /// Remove item from store
        /// </summary>
        /// <Test>
        /// TestRemoveProductFromStoreSuccess
        /// TestRemoveProductFromStoreFailureInvalid
        /// TestRemoveProductFromStoreFailureLogic
        /// </Test>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The sore id</param>
        /// <param name="productId">The product id</param>
        /// <returns>Result of the product removal</returns>
        public Result RemoveItemFromStore(string token, string storeId, string productId);
        
        /// <summary>
        /// Edit the item
        /// </summary>
        /// <Test>
        /// TestEditItemInStoreSuccess
        /// TestEditItemInStoreFailureInvalid
        /// TestEditItemInStoreFailureLogic
        /// </Test>
        /// <param name="token">Authorization token</param>
        /// <param name="item">The new item</param>
        /// <returns>Result of the edit</returns>
        public Result EditItemInStore(string token, IItem item);
        
        /// <summary>
        /// Add items amount
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="item">The item</param>
        /// <returns>Result of updating the amount (adding)</returns>
        public Result UpdateStock_AddItems(string token, IItem item);
        
        /// <summary>
        /// Add items amount
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="item">The item</param>
        /// <returns>Result of updating the amount (subtract)</returns>
        public Result UpdateStock_SubtractItems(string token, IItem item);

        /// <summary>
        /// Return all the purchase history of a store
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The storeId</param>
        /// <returns>List of the purchase history in a store</returns>
        public Result<IList<PurchaseRecord>> GetPurchaseHistoryOfStore(string token, string storeId);
        

        #endregion
    }
}