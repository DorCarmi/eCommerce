using System.Collections.Generic;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class Admin : Member
    {
        private static readonly Admin state = new Admin();

        private static readonly IList<StorePermission> Permissions = new List<StorePermission>(new []{StorePermission.ReceiveTransactionHistory});
        
        
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
        
        public override Result HasPermission(User user, IStore store, StorePermission storePermission)
        {
            if (Permissions.Contains(storePermission))
                return Result.Ok();
            return user.HasPermission(Member.State, store,storePermission);
        }
    }
}