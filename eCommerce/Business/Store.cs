using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Business.Service;
using eCommerce.Common;
using Microsoft.Extensions.Logging;

namespace eCommerce.Business
{
    public class Store : IStore
    {
        //Individual
        private String _storeName;
        
        //General
        private MarketFacade _marketFacade;

        //Store's issues
        //Discounts and purchases
        private List<DiscountStrategy> _myDiscountStrategies;
        private List<PurchaseStrategy> _myPurchaseStrategies;
        private DiscountPolicy _myDiscountPolicy;
        private PurchasePolicy _myPurchasePolicy;
        
        //History and inventory
        private StoreTransactionHistory _transactionHistory;
        private ItemsInventory _inventory;

        //User issues
        private IUser _founder;
        private List<OwnerAppointment> _ownersAppointments;
        private List<IUser> _owners;
        private List<ManagerAppointment> _managersAppointments;
        private List<IUser> _managers;
        
        private List<IBasket> _basketsOfThisStore;

        public Store(String name, IUser founder, ItemInfo item)
        {
            this._storeName = name;

            this._myDiscountStrategies = new List<DiscountStrategy>();
            this._myDiscountStrategies.Add(new DefaultDiscountStrategy());
            this._myPurchaseStrategies = new List<PurchaseStrategy>();
            this._myPurchaseStrategies.Add(new DefaultPurchaseStrategy(this));

            _myPurchasePolicy = new PurchasePolicy(this);
            _myDiscountPolicy = new DiscountPolicy(this);
            
            _transactionHistory = new StoreTransactionHistory(this);

            _inventory = new ItemsInventory(this);

            this._founder = founder;

            _owners = new List<IUser>();
            _ownersAppointments = new List<OwnerAppointment>();
            _managers = new List<IUser>();
            _managersAppointments = new List<ManagerAppointment>();
            
            _inventory.AddNewItem(founder, item);
            _basketsOfThisStore = new List<IBasket>();
        }
        
        
        
        

        public IList<Item> GetAllItems()
        {
            return this._inventory.GetAllItemsInStore();
        }
        
        public Result<Item> GetItem(ItemInfo item)
        {
            return _inventory.GetItem(item);
        }

        public Result<Item> GetItem(string itemId)
        {
            return _inventory.GetItem(itemId);
        }

        public Result<IList<Tuple<string, IList<StorePermission>>>> GetStoreStaffAndTheirPermissions(IUser user)
        {
            IList<Tuple<string, IList<StorePermission>>> netasCrazyList = new List<Tuple<string, IList<StorePermission>>>();
            foreach (var manager in _managersAppointments)
            {
                List<StorePermission> mangPerm = new List<StorePermission>();
                foreach (var perm in Enum.GetValues(typeof(StorePermission)).Cast<StorePermission>())
                {
                    if (manager.HasPermission(perm).IsSuccess)
                    {
                        mangPerm.Add(perm);
                    }
                }
                netasCrazyList.Add(new Tuple<string, IList<StorePermission>>(manager.User.Username,mangPerm));
            }
            
            foreach (var owner in _ownersAppointments)
            {
                List<StorePermission> ownerPerm = new List<StorePermission>();
                foreach (var perm in Enum.GetValues(typeof(StorePermission)).Cast<StorePermission>())
                {
                    if (owner.HasPermission(perm).IsSuccess)
                    {
                        ownerPerm.Add(perm);
                    }
                }
                netasCrazyList.Add(new Tuple<string, IList<StorePermission>>(owner.User.Username,ownerPerm));
            }
            
            List<StorePermission> foundPerm = new List<StorePermission>();
            foreach (var perm in Enum.GetValues(typeof(StorePermission)).Cast<StorePermission>())
            {
                if (this._founder.HasPermission(this,perm).IsSuccess)
                {
                    foundPerm.Add(perm);
                }
            }
            netasCrazyList.Add(new Tuple<string, IList<StorePermission>>(_founder.Username,foundPerm));


            return Result.Ok<IList<Tuple<string, IList<StorePermission>>>>(netasCrazyList);
        }

        public List<Item> SearchItem(string stringSearch)
        {
            return this._inventory.SearchItem(stringSearch);
        }

        public List<Item> SearchItemWithPriceFilter(string stringSearch, int startPrice, int endPrice)
        {
            return _inventory.SearchItemWithPriceFilter(stringSearch, startPrice, endPrice);
        }

        public List<Item> SearchItemWithCategoryFilter(string stringSearch, string category)
        {
            return _inventory.SearchItemWithCategoryFilter(stringSearch, category);
        }


        public Result AddBasketToStore(IBasket basket)
        {
            if (this._basketsOfThisStore.FirstOrDefault(x => x.GetCart() == basket.GetCart()) != null)
            {
                return Result.Fail("Store already contains basket for this cart");
            }
            else
            {
                this._basketsOfThisStore.Add(basket);
                return Result.Ok(basket);
            }
        }
        public Result CalculateBasketPrices(IBasket basket)
        {           
            foreach (var strategy in _myDiscountStrategies)
            {
                var res=basket.SetTotalPrice();
                if (res.IsFailure)
                {
                    return res;
                }
            }

            return Result.Ok();
        }

        public Result CatchAllBasketProducts(IBasket basket)
        {
            foreach (var itemInfo in basket.GetAllItems().GetValue())
            {
                var res=this._inventory.GetItem(itemInfo);
                if (res.IsFailure)
                {
                    return res;
                }
                else
                {
                    res.GetValue().AquireItems(itemInfo);
                }
                    
            }
            return Result.Ok();
            
        }

        public Result FinishPurchaseOfBasket(IBasket basket)
        {
            foreach (var item in basket.GetAllItems().GetValue())
            {
                var res = this._inventory.GetItem(item);
                if (res.IsFailure)
                {
                    return res;
                }
                else
                {
                    var resFinish=res.GetValue().FinalizeGetItems(item.amount);
                    if (resFinish.IsFailure)
                    {
                        return resFinish;
                    }
                    
                    
                }
            }

            return Result.Ok();
        }

        public Result FinishPurchaseOfItems(ItemInfo item)
        {
            var res = this._inventory.GetItem(item);
            if (res.IsFailure)
            {
                return res;
            }
            else
            {
                res.GetValue().FinalizeGetItems(item.amount);
            }
            return Result.Ok();
        }

        public Result AddItemToStore(ItemInfo newItem, IUser user)
        {
            if (user.HasPermission(this, StorePermission.AddItemToStore).IsFailure)
            {
                return Result.Fail("User has now permission to add items to store");
            }
            else
            {
                if (this._inventory.GetItem(newItem).IsFailure)
                {
                    return this._inventory.AddNewItem(user, newItem);
                }
                else
                {
                    return this._inventory.AddExistingItem(user, newItem.name, newItem.amount);
                }
            }
        }

        public Result EditItemToStore(ItemInfo newItem, IUser user)
        {
            if (user.HasPermission(this, StorePermission.AddItemToStore).IsFailure)
            {
                return Result.Fail("User has now permission to add items to store");
            }
            else
            {
                var itemGet = this._inventory.GetItem(newItem);
                if (itemGet.IsFailure)
                {
                    return Result.Fail("Item doesn't exist");
                }
                else
                {
                    return itemGet.GetValue().EditItem(newItem);
                    
                }
            }
        }

        // TODO implement this method
        public Result RemoveItemToStore(string productName, IUser user)
        {
            throw new NotImplementedException();
        }
        
        public Result RemoveItemToStore(ItemInfo newItem, IUser user)
        {
            if (user.HasPermission(this, StorePermission.AddItemToStore).IsFailure)
            {
                return Result.Fail("User has now permission to add items to store");
            }
            else
            {
                var itemGet = this._inventory.GetItem(newItem);
                if (itemGet.IsFailure)
                {
                    return Result.Fail("Item doesn't exist");
                }
                else
                {
                    return this._inventory.RemoveItem(user, newItem);
                }
            }
        }

        public Result AppointNewOwner(IUser user, OwnerAppointment ownerAppointment)
        {
            if (user.HasPermission(this, StorePermission.ControlStaffPermission).IsFailure)
            {
                return Result.Fail("User doesn't have the permission to add permissions to someone else");
            }
            else
            {
                this._ownersAppointments.Add(ownerAppointment);
                return Result.Ok();
            }
        }

        public Result AppointNewManager(IUser user, ManagerAppointment managerAppointment)
        {
            if (user.HasPermission(this, StorePermission.ControlStaffPermission).IsFailure)
            {
                return Result.Fail("User doesn't have the permission to add permissions to someone else");
            }
            else
            {
                this._managersAppointments.Add(managerAppointment);
                return Result.Ok();
            }
        }

        public Result<IList<PurchaseRecord>> GetPurchaseHistory(IUser user)
        {
            return this._transactionHistory.GetHistory(user);
        }

        public Result EnterBasketToHistory(IBasket basket)
        {
            return this._transactionHistory.AddRecordToHistory(basket);
        }

        public string GetStoreName()
        {
            return this._storeName;
        }

        public bool CheckWithPolicy(PurchaseStrategyName purchaseStrategy)
        {
            return this._myPurchasePolicy.checkStrategy(purchaseStrategy);
        }

        public bool TryAddNewCartToStore(Cart cart)
        {
            bool ans=true;
            foreach (var basket in this._basketsOfThisStore)
            {
                if (basket.GetCart() == cart)
                {
                    ans = false;
                }
            }

            return ans;

        }

        public Result ConnectNewBasketToStore(Basket newBasket)
        {
            foreach (var basket in _basketsOfThisStore)
            {
                if (basket.GetCart() == newBasket.GetCart())
                {
                    return Result.Fail("Two baskets for the same store in cart");
                }
                else
                {
                    this._basketsOfThisStore.Add(newBasket);

                }
            }
            return Result.Ok();
        }

        public bool CheckConnectionToCart(ICart cart)
        {
            foreach (var basket in _basketsOfThisStore)
            {
                if (basket.GetCart() == cart)
                {
                    return true;
                }
            }

            return false;
        }

        public Result<double> CheckDiscount(Basket basket)
        {
            //No double discounts
            double minValue = basket.GetTotalPrice().GetValue();
            foreach (var strategy in this._myDiscountStrategies)
            {
                var price = strategy.GetTotalPrice(basket);
                if (price.IsFailure)
                {
                    return price;
                }
                else
                {
                    if (price.GetValue() < minValue)
                    {
                        minValue = price.GetValue();
                    }
                }
            }

            return Result.Ok(minValue);
        }
    }
}