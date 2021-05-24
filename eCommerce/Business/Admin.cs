﻿using System.Collections.Generic;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class Admin : Member
    {
        private static readonly Admin state = new Admin();
        
        //TODO: Check with sharon about change, was AdminGetHistory
        private static readonly IList<StorePermission> Permissions = new List<StorePermission>(new []{StorePermission.GetStoreHistory});
        
        
        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit  
        static Admin(){}  
        private Admin(){}  
        public static Admin State  
        {  
            get  
            {  
                return state;  
            }  
        }
        
        /// <TEST> UserTest.TestGetStoreHistory </TEST>
        /// <UC> 'Admin requests for store history' </UC>
        /// <REQ> 6.4 </REQ>
        /// <summary>
        ///  if 'storePermission' is 'GetStoreHistory',
        ///   returns all purchase-records of the store. 
        /// </summary>
        public override Result HasPermission(User user, IStore store, StorePermission storePermission)
        {
            if (Permissions.Contains(storePermission))
                return Result.Ok();
            return user.HasPermission(Member.State, store,storePermission);
        }

        public override Result<IList<PurchaseRecord>> GetUserPurchaseHistory(User user, IUser otherUser)
        {
            return otherUser.GetUserPurchaseHistory();
        }


    }
}