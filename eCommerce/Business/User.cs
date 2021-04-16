using System.Collections.Generic;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class User : IUser
    {
        public Result Login()
        {
            throw new System.NotImplementedException();
        }

        public Result Logout()
        {
            throw new System.NotImplementedException();
        }
        

        public Result<string> OpenStore(string storeName)
        {
            throw new System.NotImplementedException();
        }

        public Result<IStore> OpenNewStore(StoreInfo storeInfo)
        {
            throw new System.NotImplementedException();
        }

        public Result AddItemToCart(Item item)
        {
            throw new System.NotImplementedException();
        }

        public Result<CartInfo> GetCartInfo()
        {
            throw new System.NotImplementedException();
        }

        public Result EditCart(ItemInfo info)
        {
            throw new System.NotImplementedException();
        }

        public Result AppointUserToOwner(IStore store, IUser user)
        {
            throw new System.NotImplementedException();
        }

        public Result AppointUserToManager(IStore store, IUser user)
        {
            throw new System.NotImplementedException();
        }

        public Result AddPermissionsToManager(IStore store, IUser user, StorePermission permission)
        {
            throw new System.NotImplementedException();
        }

        public Result RemovePermissionsToManager(IStore store, IUser user, StorePermission permission)
        {
            throw new System.NotImplementedException();
        }

        public Result<IList<IUser>> GetAllStoreStakeholders(IStore store)
        {
            throw new System.NotImplementedException();
        }

        public Result<IBasket> GetUserPurchaseHistory(IStore store)
        {
            throw new System.NotImplementedException();
        }

        public Result<IBasket> GetStorePurchaseHistory(IStore store)
        {
            throw new System.NotImplementedException();
        }

        public Result HasPermission(IStore store, StorePermission storePermission)
        {
            throw new System.NotImplementedException();
        }

        public Result EnterBasketToHistory(IBasket basket)
        {
            throw new System.NotImplementedException();
        }

                        
        // TODO check with the implementor
        public string UserId { get; set; }
        
        public string Username { get; }
        
        public void Connect()
        {
            throw new System.NotImplementedException();
        }

        public bool Disconnect()
        {
            throw new System.NotImplementedException();
        }

        // For guest
        public User(string username)
        {
            
        }
        
        public User(MemberInfo memberInfo)
        {
            
        }
    }
}