using System;
using System.Collections.Generic;
using eCommerce.Business.Discounts;
using eCommerce.Common;

namespace eCommerce.Business.Service
{
    public interface IStore
    {
        
        /// <TEST>StoreGetAllItems</TEST>
        /// <UC>StoreGetAllItems</UC>
        /// <REQ>StoreGetAllItems</REQ>
        /// <summary>
        /// Ask store to return all instances of items belongs to store
        /// </summary>
        /// <returns>Collection of items belongs to store</returns>
        public IList<Item> GetAllItems();

        public Result<Item> GetItem(ItemInfo item);

        public Result TryGetItems(ItemInfo item);
        
        // TODO implement
        public Result<Item> GetItem(string itemId);
        
        // TODO implement, return list of username and there permissions
        public Result<IList<Tuple<string, IList<StorePermission>>>> GetStoreStaffAndTheirPermissions(IUser user);

        public Result<IList<StorePermission>> GetPermissions(IUser user);

        public List<Item> SearchItem(string stringSearch);

        public List<Item> SearchItemWithPriceFilter(string stringSearch, int startPrice, int endPrice);

        public List<Item> SearchItemWithCategoryFilter(string stringSearch, string category);

        public Result AddBasketToStore(IBasket basket);

        public Result CalculateBasketPrices(IBasket basket);

        public Result CatchAllBasketProducts(IBasket basket);

        public Result FinishPurchaseOfBasket(IBasket basket);
        public Result FinishPurchaseOfItems(ItemInfo itemInfo);


        public Result AddItemToStore(ItemInfo newItem, IUser user);
        public Result EditItemToStore(ItemInfo newItem, IUser user);
        
        public Result UpdateStock_AddItems(ItemInfo newItem, IUser user);
        
        public Result UpdateStock_SubtractItems(ItemInfo newItem, IUser user);
        
        
        public Result RemoveItemToStore(ItemInfo newItem, IUser user);
        public Result RemoveItemToStore(string itemID, IUser user);

        public Result AppointNewOwner(IUser user, OwnerAppointment ownerAppointment);
        public Result AppointNewManager(IUser user, ManagerAppointment managerAppointment);

        public Result<IUser> GetFounder();

        public Result RemoveOwnerFromStore(IUser theOneWhoFires, IUser theFierd, OwnerAppointment ownerAppointment);

        public Result RemoveManagerFromStore(IUser theOneWhoFires, IUser theFired,
            ManagerAppointment managerAppointment);

        public Result<IList<PurchaseRecord>> GetPurchaseHistory(IUser user);
        public Result EnterBasketToHistory(IBasket basket);


        public String GetStoreName();
        bool CheckWithPolicy(PurchaseStrategyName purchaseStrategy);
        bool TryAddNewCartToStore(ICart cart);
        Result ConnectNewBasketToStore(IBasket newBasket);
        bool CheckConnectionToCart(ICart cart);
        
        Result<PurchaseRecord> AddBasketRecordToStore(Basket basket);
        
        
        
        Result<double> CheckDiscount(Basket basket);
        Result AddPurchaseStrategyToStore(IUser user, PurchaseStrategyName purchaseStrategy);
        Result<IList<PurchaseStrategyName>> GetStorePurchaseStrategy(IUser user);
        Result UpdatePurchaseStrategies(IUser user, PurchaseStrategyName purchaseStrategy);
        Result AddPurchaseStrategyToStoreItem(IUser user, string storeId, string itemId, PurchaseStrategyName strategyName);
        Result RemovePurchaseStrategyToStoreItem(IUser user, string storeId, string itemId, PurchaseStrategyName strategyName);
        Result<IList<PurchaseStrategyName>> GetPurchaseStrategyToStoreItem(IUser user, string storeId, string itemId, PurchaseStrategyName strategyName);


        Result AddDiscountToStore(DiscountInfoNode infoNode);
    }
}