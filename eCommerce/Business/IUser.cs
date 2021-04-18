﻿using System.Collections.Generic;
using System.ComponentModel;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public interface IUser
    {
        //Facade
        public Result OpenStore(IStore store);
        public Result AddItemToCart(ItemInfo item);
        public Result<ICart> GetCartInfo();
        public Result EditCart(ItemInfo info);

        public Result AppointUserToOwner(IStore store, IUser user);
        public Result AppointUserToManager(IStore store, IUser user);
        
        public Result<OwnerAppointment> MakeOwner(IStore store);
        public Result<ManagerAppointment> MakeManager(IStore store);

        public Result UpdatePermissionsToManager(IStore store, IUser user, IList<StorePermission> permissions);

        // TODO implement
        public Result<IList<PurchaseRecord>> GetUserPurchaseHistory(IStore store);
        public Result<IList<PurchaseRecord>> GetUserPurchaseHistory(IStore store, IUser otherUser);

        public Result<IList<PurchaseRecord>>  GetStorePurchaseHistory(IStore store);

        
        
        //InBusiness
        public Result HasPermission(IStore store, StorePermission storePermission);
        public Result EnterBasketToHistory(IBasket basket);
        
        // Added
        public string Username { get; }







    }

    
}