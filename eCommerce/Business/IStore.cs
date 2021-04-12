﻿using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Business.Service
{
    public interface IStore
    {
        public IList<Item> GetAllItems();
        public IList<StoreInfo> GetAllStores();
        
        
        public Result<Item> GetItem();
        public IList<Item> SearchForItems(string stringSearch);
        public IList<Item> SearchForItems(string categoryName);
        public IList<Item> SearchForItems(string[] keyWords);
        public IList<Item> SearchForItems(string[] keyWords);
        public IList<Item> SearchForItems(string[] keyWords);
        public IList<Item> SearchForItems(string stringSearch);
        public IList<Store> SearchForStore(string stringSearch);

        public Result AddBasketToStore(IBasket basket);

        public Result<CartInfo> ShowCart();

        public Result CalculateCartPrices();

        public Result<int> CatchAllBasketProducts(IBasket basket);

        public Result FinishPurchaseOfBasket(IBasket basket);


        public Result AddItemToStore(ItemInfo newItem, User user);

        public Result AppointNewOwner(IUser user, OwnerAppointment ownerAppointment);
        public Result AppointNewManager(IUser user, OwnerAppointment ownerAppointment);

        public Result<IList<IBasket>> GetPurchaseHistory();
        public Result EnterBasketToHistory(IBasket basket);


    }
}