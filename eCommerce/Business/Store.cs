using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Business.Basics;
using Microsoft.Extensions.Logging;

namespace eCommerce.Business
{
    public class Store
    {
        //Individual
        private readonly String _storeName;
        
        //General
        private MarketFacade _marketFacade;
        
        //Inventory
        private ItemsInventory _invetory;

        //Store's issues
        //Discounts and purchases
        private List<DiscountStrategy> _myDiscountStrategies;
        private List<PurchaseStrategy> _myPurchaseStrategies;
        private DiscountPolicy _myDiscountPolicy;
        private PurchasePolicy _myPurchasePolicy;
        
        //History and inventory
        private StoreTransactionHistory _transactionHistory;


        //User issues
        private User _founder;
        private List<OwnerAppointment> _ownersAppoinements;
        private List<User> _owners;
        private List<ManagerAppointment> _managersAppointments;
        private List<User> _managers;
        
        private List<Basket> _basketsOfThisStore;

        public Store(String name, MarketFacade facade, User founder, ItemInfo item)
        {
            _storeName = name;
            _marketFacade = facade;
            _invetory = new ItemsInventory(this);
            _invetory.addNewItem(item);
            _myDiscountStrategies = new List<DiscountStrategy>();
            _myDiscountStrategies.Add(new DefaultDiscountStrategy());
            _myPurchaseStrategies = new List<PurchaseStrategy>();
            _myPurchaseStrategies.Add(new DefaultPurchasePolicy());
            _myDiscountPolicy = new DiscountPolicy();
            _myPurchasePolicy = new PurchasePolicy();
            _transactionHistory = new StoreTransactionHistory();
            _founder = founder;
            _ownersAppoinements = new List<OwnerAppointment>();
            _owners = new List<User>();
            _managersAppointments = new List<ManagerAppointment>();
            _managers = new List<User>();
            _basketsOfThisStore = new List<Basket>();
            
            facade.addNewStore(this);
        }
        public String getStoreName()
        {
            return this._storeName;
        }

        public Answer<bool> addNewItemToStore(User user,ItemInfo item)
        {
            if (user.hasPermission(this, StorePermission.AddItemToStore))
            {
                return _invetory.addNewItem(item);
            }
            else
            {
                return new Answer<bool>("Has no permissions to add item to store");
            }
        }

        public Answer<bool> addItemsToStore(String itemName, int amount)
        {
            return _invetory.addExistingItem(itemName, amount);
        }

        public Answer<Item> getItemsToBasket(String itemName, int amount)
        {
            return _invetory.getItemsToBasket(itemName, amount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public Answer<List<ItemInfo>> searchForStoreItems(String searchString)
        {
            return _invetory.findItems(searchString);
        }

        

        public bool checkWithStorePolicy(PurchaseStrategies strategyName)
        {
            bool ans=this._myPurchasePolicy.checkStrategy(strategyName);
            ans = ans && this._myPurchaseStrategies.Select(x => x.getStrategyName() == strategyName).Count() > 0;
            return ans;
        }

        public bool checkWithStorePolicy(int pricePerUnit, int amount)
        {
            return _myPurchasePolicy.checkAmountAndPrice(pricePerUnit, amount);
        }

        public PurchaseStrategy getPurchaseStrategy(PurchaseStrategies strategyName)
        {
            var strategy= this._myPurchaseStrategies.FirstOrDefault(x => x.getStrategyName() == strategyName);
            if (strategy==null)
            {
                return new DefaultPurchasePolicy();
            }
            else
            {
                return strategy;
            }
            
        }
    }
}