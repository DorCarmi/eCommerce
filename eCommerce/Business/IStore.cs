﻿using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Business.Service
{
    public interface IStore
    {
        /// <summary>
        /// <CNAME>StoreGetAllItems</CNAME>
        /// </summary>
        /// <returns></returns>
        public IList<Item> GetAllItems();
        public IList<StoreInfo> GetAllStores();
        
        
        public Result<Item> GetItem(string itemId);
        public IList<Item> SearchForItems1(string stringSearch);
        public IList<Item> SearchForItems2(string categoryName);
        public IList<Item> SearchForItems3(string[] keyWords);
        public IList<Item> SearchForItems4(string[] keyWords);
        public IList<Item> SearchForItems5(string[] keyWords);
        public IList<ItemInfo> SearchForItems(string stringSearch);
        public IList<Store> SearchForStore(string stringSearch);

        public Result AddBasketToStore(IBasket basket);

        public Result<CartInfo> ShowCart();

        public Result CalculateCartPrices();

        public Result<int> CatchAllBasketProducts(IBasket basket);

        public Result FinishPurchaseOfBasket(IBasket basket);


        public Result AddItemToStore(ItemInfo newItem, IUser user);
        
        public Result EditProduct(ItemInfo item, IUser user);
        
        public Result RemoveProduct(string productId, IUser user);



        public Result AppointNewOwner(IUser user, OwnerAppointment ownerAppointment);
        public Result AppointNewManager(IUser user, OwnerAppointment ownerAppointment);

        public Result<IList<IBasket>> GetPurchaseHistory();
        public Result EnterBasketToHistory(IBasket basket);

        public Result<IList<StaffPermission>> GetStaffPermissions(IUser requester);
        
        // TODO check with the implementor
        
        public string StoreName
        {
            get;
        }
    }
}