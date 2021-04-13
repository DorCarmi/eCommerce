using System.Collections.Generic;
using System.ComponentModel;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public interface IUser
    {
        //Facade
        public Result Login(UserToSystemState systemState);
        public Result Logout();
        public Result<string> OpenStore(string storeName);
        public Result<IStore> OpenNewStore(StoreInfo storeInfo);


        public Result AddItemToCart(Item item);
        public Result<CartInfo> GetCartInfo();
        public Result EditCart(ItemInfo info);

        public Result AppointUserToOwner(IStore store, IUser user);
        public Result AppointUserToManager(IStore store, IUser user);

        public Result AddPermissionsToManager(IStore store, IUser user, StorePermission permission);
        public Result RemovePermissionsToManager(IStore store, IUser user, StorePermission permission);

        public Result<IList<IUser>> GetAllStoreStakeholders(IStore store);
        public Result<IBasket> GetUserPurchaseHistory(IStore store);
        public Result<IBasket> GetStorePurchaseHistory(IStore store);
        
        
        //InBusiness
        public Result HasPermission(IStore store, StorePermission storePermission);
        public Result EnterBasketToHistory(IBasket basket);








    }

    
}