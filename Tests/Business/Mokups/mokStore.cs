using System;
using System.Collections.Generic;
using eCommerce.Business;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace Tests.Business.Mokups
{
    public class mokStore: IStore
    {
        public string storeName { get; set; }
        public Item item;
        public ICart cart;

        public mokStore(string storeName)
        {
            this.storeName = storeName;
        }
        
        public IList<Item> GetAllItems()
        {
            throw new System.NotImplementedException();
        }

        public Result<Item> GetItem(ItemInfo item)
        {
            return Result.Ok<Item>(this.item);
        }

        public Result<Item> GetItem(string itemId)
        {
            throw new System.NotImplementedException();
        }

        public Result<IList<Tuple<string, IList<StorePermission>>>> GetStoreStaffAndTheirPermissions(IUser user)
        {
            throw new NotImplementedException();
        }

        public List<Item> SearchItem(string stringSearch)
        {
            throw new System.NotImplementedException();
        }

        public List<Item> SearchItemWithPriceFilter(string stringSearch, int startPrice, int endPrice)
        {
            throw new System.NotImplementedException();
        }

        public List<Item> SearchItemWithCategoryFilter(string stringSearch, string category)
        {
            throw new System.NotImplementedException();
        }

        public Result AddBasketToStore(IBasket basket)
        {
            throw new System.NotImplementedException();
        }

        public Result CalculateBasketPrices(IBasket basket)
        {
            throw new System.NotImplementedException();
        }

        public Result CatchAllBasketProducts(IBasket basket)
        {
            throw new System.NotImplementedException();
        }

        public Result FinishPurchaseOfBasket(IBasket basket)
        {
            throw new System.NotImplementedException();
        }

        public Result FinishPurchaseOfItems(ItemInfo itemInfo)
        {
            throw new System.NotImplementedException();
        }

        public Result AddItemToStore(ItemInfo newItem, IUser user)
        {
            throw new System.NotImplementedException();
        }

        public Result EditItemToStore(ItemInfo newItem, IUser user)
        {
            throw new System.NotImplementedException();
        }

        public Result UpdateStock_AddItems(ItemInfo newItem, IUser user)
        {
            throw new NotImplementedException();
        }

        public Result UpdateStock_SubtractItems(ItemInfo newItem, IUser user)
        {
            throw new NotImplementedException();
        }

        public Result RemoveItemToStore(ItemInfo newItem, IUser user)
        {
            throw new System.NotImplementedException();
        }

        public Result RemoveItemToStore(string itemID, IUser user)
        {
            throw new NotImplementedException();
        }

        public Result AppointNewOwner(IUser user, OwnerAppointment ownerAppointment)
        {
            Console.WriteLine("MokStore: "+user.Username+" Appointed new Owner: "+ownerAppointment.User.Username);
            return Result.Ok();
        }

        public Result AppointNewManager(IUser user, ManagerAppointment managerAppointment)
        {
            Console.WriteLine("MokStore: "+user.Username+" Appointed new Manager: "+managerAppointment.User.Username);
            return Result.Ok();
        }

        public Result<IList<PurchaseRecord>> GetPurchaseHistory(IUser user)
        {
            Console.WriteLine("MokStore: Getting Purchase History (its empty..)");
            return Result.Ok<IList<PurchaseRecord>>(new List<PurchaseRecord>());
        }

        public Result EnterBasketToHistory(IBasket basket)
        {
            Console.WriteLine("MokStore: Entered Basket to History.");
            return  Result.Ok();
        }

        public string GetStoreName()
        {
            return this.storeName;
        }

        public bool CheckWithPolicy(PurchaseStrategyName purchaseStrategy)
        {
            throw new System.NotImplementedException();
        }

        public bool TryAddNewCartToStore(Cart cart)
        {
            Console.WriteLine("MokStore: Added New Cart To Store");
            return true;
        }

        public Result ConnectNewBasketToStore(Basket newBasket)
        {
            Console.WriteLine("MockStore: Connected new basket to store");
            return Result.Ok();
        }

        public bool CheckConnectionToCart(ICart cart)
        {
            return cart == this.cart;
        }

        public Result<double> CheckDiscount(Basket basket)
        {
            throw new System.NotImplementedException();
        }

        public Result AddPurchaseStrategyToStore(IUser user, PurchaseStrategyName purchaseStrategy)
        {
            throw new NotImplementedException();
        }

        public Result<IList<PurchaseStrategyName>> GetStorePurchaseStrategy(IUser user)
        {
            throw new NotImplementedException();
        }

        public Result UpdatePurchaseStrategies(IUser user, PurchaseStrategyName purchaseStrategy)
        {
            throw new NotImplementedException();
        }

        public Result AddPurchaseStrategyToStoreItem(IUser user, string storeId, string itemId, PurchaseStrategyName strategyName)
        {
            throw new NotImplementedException();
        }

        public Result RemovePurchaseStrategyToStoreItem(IUser user, string storeId, string itemId, PurchaseStrategyName strategyName)
        {
            throw new NotImplementedException();
        }

        public Result<IList<PurchaseStrategyName>> GetPurchaseStrategyToStoreItem(IUser user, string storeId, string itemId, PurchaseStrategyName strategyName)
        {
            throw new NotImplementedException();
        }
    }
}