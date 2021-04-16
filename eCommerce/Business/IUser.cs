﻿using System.Collections.Generic;
using System.ComponentModel;
using eCommerce.Business.Service;
using eCommerce.Common;
using Microsoft.AspNetCore.SignalR;

namespace eCommerce.Business
{
    public interface IUser
    {
        //Facade
        public Result Login();

        public Result Logout();

        public Result<string> OpenStore(string storeName);
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
        
        // Added
        // TODO check with the implementer
        public string UserId { get; set; }
        public string Username { get; }
        public void Connect();
        /// <summary>
        /// Discount the user
        /// </summary>
        /// <returns>If the user was registered or not</returns>
        public bool Disconnect();
    }

    
}