using System;
using System.Collections.Generic;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Service
{
    public interface IStoreService
    {
        /// <summary>
        /// Get all the staff of the store and their permissions
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The storeId</param>
        /// <returns>List of all the staff and their permissions</returns>
        public Result<IList<StaffPermission>> GetStoreStaffAndTheirPermissions(string token, string storeId);
        
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
        public Result<ServiceStore> GetStore(string token, string storeId);
        
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
    }
}