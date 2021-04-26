using System.Collections.Generic;
using eCommerce.Business;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Service
{
    public interface IUserService
    {
               
        /// <summary>
        /// Get the purchase history of the user 
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>The purchase history</returns>
        public Result<SPurchaseHistory> GetPurchaseHistory(string token);
        
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
        
        /// <summary>
        /// Get the history purchase of a user
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <param name="ofUserId">The user id</param>
        /// <returns>The history purchase</returns>
        public Result<SPurchaseHistory> AdminGetPurchaseHistoryUser(string token, string ofUserId);
        
        /// <summary>
        /// Get the history purchase of a store
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="storeId">The store id</param>
        /// <returns>The history purchase</returns>
        public Result<SPurchaseHistory> AdminGetPurchaseHistoryStore(string token, string storeID);
    }
}