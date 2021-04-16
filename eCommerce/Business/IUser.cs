using System.Collections.Generic;
using System.ComponentModel;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public interface IUser
    {
        //Facade
        public Result Login(UserToSystemState systemState, MemberData memberData);
        public Result Logout(string toGuestName);
        public Result<bool> IsRegistered();
        public Result OpenStore(IStore store);


        public Result AddItemToCart(Item item);
        public Result<CartInfo> GetCartInfo();
        public Result EditCart(ItemInfo info);

        public Result AppointUserToOwner(IStore store, IUser user);
        public Result AppointUserToManager(IStore store, IUser user);
        
        public Result MakeOwner(IStore store);
        public Result MakeManager(IStore store);

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