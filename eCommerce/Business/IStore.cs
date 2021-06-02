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
        public Result<IList<Tuple<string, IList<StorePermission>>>> GetStoreStaffAndTheirPermissions(User user);

        public Result<IList<StorePermission>> GetPermissions(User user);

        public List<Item> SearchItem(string stringSearch);

        public List<Item> SearchItemWithPriceFilter(string stringSearch, int startPrice, int endPrice);

        public List<Item> SearchItemWithCategoryFilter(string stringSearch, string category);

        public Result AddBasketToStore(IBasket basket);

        public Result CalculateBasketPrices(IBasket basket);

        public Result CatchAllBasketProducts(IBasket basket);

        public Result FinishPurchaseOfBasket(IBasket basket);
        public Result FinishPurchaseOfItems(ItemInfo itemInfo);


        public Result AddItemToStore(ItemInfo newItem, User user);
        public Result EditItemToStore(ItemInfo newItem, User user);
        
        public Result UpdateStock_AddItems(ItemInfo newItem, User user);
        
        public Result UpdateStock_SubtractItems(ItemInfo newItem, User user);
        
        
        public Result RemoveItemToStore(ItemInfo newItem, User user);
        public Result RemoveItemToStore(string itemID, User user);

        public Result AppointNewOwner(User user, OwnerAppointment ownerAppointment);
        public Result AppointNewManager(User user, ManagerAppointment managerAppointment);

        public Result<User> GetFounder();

        public Result RemoveOwnerFromStore(User theOneWhoFires, User theFierd, OwnerAppointment ownerAppointment);

        public Result RemoveManagerFromStore(User theOneWhoFires, User theFired,
            ManagerAppointment managerAppointment);

        public Result<IList<PurchaseRecord>> GetPurchaseHistory(User user);
        public Result EnterBasketToHistory(IBasket basket);


        public String GetStoreName();

        bool TryAddNewCartToStore(ICart cart);
        Result ConnectNewBasketToStore(IBasket newBasket);
        bool CheckConnectionToCart(ICart cart);
        
        Result<PurchaseRecord> AddBasketRecordToStore(Basket basket);
        
        
        
        Result<double> CheckDiscount(Basket basket);
        Result CheckWithStorePolicy(IBasket basket, User user);


        Result AddDiscountToStore(User user,DiscountInfoNode infoNode);
        Result AddRuleToStorePolicy(User user,RuleInfoNode ruleInfoNode);
        Result<IList<RuleInfoNode>> GetStorePolicy(User user);
        Result<IList<DiscountInfoNode>> GetStoreDiscounts(User user);

        Result ResetStorePolicy(User user);
        Result ResetStoreDiscount(User user);
    }
}