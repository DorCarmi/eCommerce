using System;
using System.Collections.Generic;
using eCommerce.Business;
using eCommerce.Business.Discounts;
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

        public Result TryGetItems(ItemInfo item)
        {
            return Result.Ok();
        }

        public Result<Item> GetItem(string itemId)
        {
            throw new System.NotImplementedException();
        }

        public Result<IList<Tuple<string, IList<StorePermission>>>> GetStoreStaffAndTheirPermissions(User user)
        {
            throw new NotImplementedException();
        }

        public Result<IList<StorePermission>> GetPermissions(User user)
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

        public Result AddItemToStore(ItemInfo newItem, User user)
        {
            throw new System.NotImplementedException();
        }

        public Result EditItemToStore(ItemInfo newItem, User user)
        {
            throw new System.NotImplementedException();
        }

        public Result UpdateStock_AddItems(ItemInfo newItem, User user)
        {
            throw new NotImplementedException();
        }

        public Result UpdateStock_SubtractItems(ItemInfo newItem, User user)
        {
            throw new NotImplementedException();
        }

        public Result RemoveItemToStore(ItemInfo newItem, User user)
        {
            throw new System.NotImplementedException();
        }

        public Result RemoveItemToStore(string itemID, User user)
        {
            throw new NotImplementedException();
        }

        public Result AppointNewOwner(User user, OwnerAppointment ownerAppointment)
        {
            Console.WriteLine("MokStore: "+user.Username+" Appointed new Owner: "+ownerAppointment.User.Username);
            return Result.Ok();
        }

        public Result AppointNewManager(User user, ManagerAppointment managerAppointment)
        {
            Console.WriteLine("MokStore: "+user.Username+" Appointed new Manager: "+managerAppointment.User.Username);
            return Result.Ok();
        }

        public Result<User> GetFounder()
        {
            throw new NotImplementedException();
        }

        public Result RemoveOwnerFromStore(User theOneWhoFires, User theFierd, OwnerAppointment ownerAppointment)
        {
            Console.WriteLine("MokStore: "+theOneWhoFires.Username+" removed the Owner: "+theFierd.Username);
            return Result.Ok();
        }

        public Result RemoveManagerFromStore(User theOneWhoFires, User theFired, ManagerAppointment managerAppointment)
        {
            throw new NotImplementedException();
        }

        public Result<IList<PurchaseRecord>> GetPurchaseHistory(User user)
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

        public bool TryAddNewCartToStore(ICart cart)
        {
            Console.WriteLine("MokStore: Added New Cart To Store");
            return true;
        }

        public Result ConnectNewBasketToStore(IBasket newBasket)
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

        public Result CheckWithStorePolicy(IBasket basket, User user)
        {
            throw new NotImplementedException();
        }

        public Result AddDiscountToStore(User user, DiscountInfoNode infoNode)
        {
            throw new NotImplementedException();
        }

        public Result AddRuleToStorePolicy(User user, RuleInfoNode ruleInfoNode)
        {
            throw new NotImplementedException();
        }

        public Result<IList<RuleInfoNode>> GetStorePolicy(User user)
        {
            throw new NotImplementedException();
        }

        public Result<IList<DiscountInfoNode>> GetStoreDiscounts(User user)
        {
            throw new NotImplementedException();
        }

        public Result ResetStorePolicy(User user)
        {
            throw new NotImplementedException();
        }

        public Result ResetStoreDiscount(User user)
        {
            throw new NotImplementedException();
        }

        public Result AddPurchaseStrategyToStore(User user, PurchaseStrategyName purchaseStrategy)
        {
            throw new NotImplementedException();
        }

        public Result<IList<PurchaseStrategyName>> GetStorePurchaseStrategy(User user)
        {
            throw new NotImplementedException();
        }

        public Result UpdatePurchaseStrategies(User user, PurchaseStrategyName purchaseStrategy)
        {
            throw new NotImplementedException();
        }

        public Result AddPurchaseStrategyToStoreItem(User user, string storeId, string itemId, PurchaseStrategyName strategyName)
        {
            throw new NotImplementedException();
        }

        public Result RemovePurchaseStrategyToStoreItem(User user, string storeId, string itemId, PurchaseStrategyName strategyName)
        {
            throw new NotImplementedException();
        }

        public Result<IList<PurchaseStrategyName>> GetPurchaseStrategyToStoreItem(User user, string storeId, string itemId, PurchaseStrategyName strategyName)
        {
            throw new NotImplementedException();
        }

        public Result AddDiscountToStore(DiscountInfoNode infoNode)
        {
            throw new NotImplementedException();
        }

        public Result<PurchaseRecord> AddBasketRecordToStore(Basket basket)
        {
            throw new NotImplementedException();
        }
    }
}