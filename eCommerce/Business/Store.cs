using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.DiscountsAndPurchases.Purchases.RulesInfo;
using eCommerce.Business.Purchases;
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
        private List<Composite> _myDiscountStrategies;
        private List<PurchaseStrategy> _myPurchaseStrategies;
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

        public Store(String name, IUser founder)
        {
            this._storeName = name;

            this._myDiscountStrategies = new List<Composite>();
            this._myPurchaseStrategies = new List<PurchaseStrategy>();
            this._myPurchaseStrategies.Add(new DefaultPurchaseStrategy(this));

            _myPurchasePolicy = new PurchasePolicy(this);

            _transactionHistory = new StoreTransactionHistory(this);

            _inventory = new ItemsInventory(this);

            this._founder = founder;

            _owners = new List<IUser>();
            _ownersAppointments = new List<OwnerAppointment>();
            
            _managers = new List<IUser>();
            _managersAppointments = new List<ManagerAppointment>();
            
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

        public Result TryGetItems(ItemInfo item)
        {
            return this._inventory.TryGetItems(item);
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
                    return Result.Fail("Item already exist in store");
                    //return this._inventory.AddExistingItem(user, newItem.name, newItem.amount);
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

        public Result UpdateStock_AddItems(ItemInfo newItem, IUser user)
        {
            if (user.HasPermission(this, StorePermission.AddItemToStore).IsFailure)
            {
                return Result.Fail("User has now permission to add items to store");
            }
            else
            {
                if (this._inventory.GetItem(newItem).IsFailure)
                {
                    return Result.Fail("Item doesn't exist in store");
                }
                else
                {
                    return this._inventory.AddExistingItem(user, newItem.name, newItem.amount);
                }
            }
        }

        public Result UpdateStock_SubtractItems(ItemInfo newItem, IUser user)
        {
            if (user.HasPermission(this, StorePermission.AddItemToStore).IsFailure)
            {
                return Result.Fail("User has now permission to add items to store");
            }
            else
            {
                if (this._inventory.GetItem(newItem).IsFailure)
                {
                    return Result.Fail("Item doesn't exist in store");
                }
                else
                {
                    return this._inventory.SubtractItems(user, newItem.name, newItem.amount);
                }
            }
        }

        // TODO implement this method
        public Result RemoveItemToStore(string productName, IUser user)
        {
            var res=this._inventory.GetItem(productName);
            if (res.IsFailure)
            {
                return res;
            }
            else
            {
                return RemoveItemToStore(res.GetValue().ShowItem(), user);
            }
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

        public Result RemoveOwnerFromStore(IUser theOneWhoFires, IUser theFired, OwnerAppointment ownerAppointment)
        {
            if (theOneWhoFires.HasPermission(this, StorePermission.RemoveStoreStaff).IsSuccess)
            {
                if (this._ownersAppointments.Contains(ownerAppointment))
                {
                    this._ownersAppointments.Remove(ownerAppointment);
                    return Result.Ok();
                }
                else
                {
                    return Result.Fail("Owner appointed not record in store");
                }
            }
            else
            {
                return Result.Fail("User doesn't have permissions to remove staff");
            }
        }
        
        
        public Result RemoveManagerFromStore(IUser theOneWhoFires, IUser theFired, ManagerAppointment managerAppointment)
        {
            if (theOneWhoFires.HasPermission(this, StorePermission.RemoveStoreStaff).IsSuccess)
            {
                if (this._managersAppointments.Contains(managerAppointment))
                {
                    this._managersAppointments.Remove(managerAppointment);
                    return Result.Ok();
                }
                else
                {
                    return Result.Fail("Manager appointed not record in store");
                }
            }
            else
            {
                return Result.Fail("User doesn't have permissions to remove staff");
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
        
        public bool TryAddNewCartToStore(ICart cart)
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

        public Result ConnectNewBasketToStore(IBasket newBasket)
        {
            if (_basketsOfThisStore.Count == 0)
            {
                this._basketsOfThisStore.Add(newBasket);
                return Result.Ok();
            }
            else
            {
                foreach (var basket in _basketsOfThisStore)
                {
                    if (basket.GetCart() == newBasket.GetCart())
                    {
                        return Result.Fail("Two baskets for the same store in cart");
                    }
                }
                this._basketsOfThisStore.Add(newBasket);
                return Result.Ok();
            }
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
            
            double minValue = basket.GetRegularTotalPrice();
            foreach (var discount in this._myDiscountStrategies)
            {
                Dictionary<string, ItemInfo> itemsInDiscount = new Dictionary<string, ItemInfo>();
                var checkIfDiscount = discount.CheckIfDiscount();
                if (checkIfDiscount)
                {
                    var checkDiscount = discount.Check(basket, basket.GetCart().GetUser());
                    if (checkDiscount.Count>0)
                    {
                        
                        var price = discount.Get(basket, basket.GetCart().GetUser());
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
                }
            }
            return Result.Ok(minValue);
        }

        public Result CheckWithStorePolicy(IBasket basket, IUser user)
        {
            return this._myPurchasePolicy.CheckWithStorePolicy(basket, user);
        }


        public Result AddDiscountToStore(IUser user,DiscountInfoNode infoNode)
        {
            if (!user.HasPermission(this, StorePermission.EditStorePolicy).IsSuccess)
            {
                return Result.Fail("User doesn't have the permission to handle store discounts and policies");
            }
            var discountRes = DiscountHandler.HandleDiscount(infoNode);
            if (discountRes.IsFailure)
            {
                return discountRes;
            }
            this._myDiscountStrategies.Add(discountRes.Value);
            return Result.Ok();
        }

        public Result AddRuleToStorePolicy(IUser user,RuleInfoNode ruleInfoNode)
        {
            var rule = RuleHandler.HandleRule(ruleInfoNode);
            if (rule.IsFailure)
            {
                return rule;
            }
            return this._myPurchasePolicy.AddRuleToStorePolicy(user, rule.Value);
        }

        public Result<IList<RuleInfoNode>> GetStorePolicy(IUser user)
        {
            return this._myPurchasePolicy.GetPolicy(user);
        }

        public Result<IList<DiscountInfoNode>> GetStoreDiscounts(IUser user)
        {
            if (!user.HasPermission(this, StorePermission.EditStorePolicy).IsSuccess)
            {
                return Result.Fail<IList<DiscountInfoNode>>("User doesn't have the permission to handle store discounts and policies");
            }
            IList<DiscountInfoNode> discountInfoNodes = new List<DiscountInfoNode>();
            foreach (var discount in this._myDiscountStrategies)
            {
                if (!discount.CheckIfDiscount())
                {
                    return Result.Fail<IList<DiscountInfoNode>>("Problem with one of the discounts in store");
                }

                var res = discount.GetDisocuntInfo();
                if (res.IsFailure)
                {
                    return Result.Fail<IList<DiscountInfoNode>>(res.Error);
                }
                discountInfoNodes.Add(res.Value);
            }

            return Result.Ok(discountInfoNodes);
        }

        public Result ResetStorePolicy(IUser user)
        {
            if (!user.HasPermission(this, StorePermission.EditStorePolicy).IsSuccess)
            {
                return Result.Fail<IList<DiscountInfoNode>>("User doesn't have the permission to handle store discounts and policies");
            }

            return this._myPurchasePolicy.Reset(user);
        }

        public Result ResetStoreDiscount(IUser user)
        {
            if (!user.HasPermission(this, StorePermission.EditStorePolicy).IsSuccess)
            {
                return Result.Fail<IList<DiscountInfoNode>>("User doesn't have the permission to handle store discounts and policies");
            }

            this._myDiscountStrategies = new List<Composite>();
            return Result.Ok();
        }

        public Result<PurchaseRecord> AddBasketRecordToStore(Basket basket)
        {
            var purchaseRecord=this._transactionHistory.AddRecordToHistory(basket);
            foreach (var owner in _owners)
            {
                foreach (var item in purchaseRecord.Value.BasketInfo._itemsInBasket)
                {
                    owner.PublishMessage(String.Format("User: {0} , bought {1} items of {2} at {3}",
                        purchaseRecord.Value.Username, item.name, item.name, purchaseRecord.Value.GetDate()));
                }
            }
            return purchaseRecord;

        }

        public Result<IUser> GetFounder()
        {
            return Result.Ok(this._founder);
        }
    }
}