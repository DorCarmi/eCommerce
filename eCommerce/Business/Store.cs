using System;
using System.Collections.Generic;
using eCommerce.Business.Basics;
using Microsoft.Extensions.Logging;

namespace eCommerce.Business
{
    public class Store
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
        private Dictionary<String, Item> _itemsInStore;
        private Dictionary<String, int> _amountOfItemsInStore;
        private Dictionary<String, List<Item>> _aquiredItemsInStore;
        
        
        
        //User issues
        private User _founder;
        private List<OwnerAppointment> _ownersAppoinements;
        private List<User> _owners;
        private List<ManagerAppointment> _managersAppointments;
        private List<User> _managers;
        
        private List<Basket> _basketsOfThisStore;

        public Store(String name, MarketFacade facade, User founder, Item item)
        {
            this._storeName = name;
            
            this._marketFacade = facade;

            this._myDiscountStrategies = new List<DiscountStrategy>();
            this._myDiscountStrategies.Add(new DefaultDiscountStrategy());
            this._myPurchaseStrategies = new List<PurchaseStrategy>();
            this._myPurchaseStrategies.Add(new DefaultPurchasePolicy());
            
            _myPurchasePolicy = new PurchasePolicy();
            _myDiscountPolicy = new DiscountPolicy();
            
            _transactionHistory = new StoreTransactionHistory();

            _itemsInStore = new Dictionary<string, List<Item>>();
            _itemsInStore.Add(item.getName(),new List<Item>{item});
            _aquiredItemsInStore = new Dictionary<string, List<Item>>();
            
            

            this._founder = founder;

            _owners = new List<OwnerAppointment>();
            _managers = new List<ManagerAppointment>();

            _basketsOfThisStore = new List<Basket>();

            facade.addNewStore(this);
        }

        public Dictionary<Item,int> getItemsToBasket(String itemID, int amount)
        {
            
        }
        
        
        public void AddItemsToStore(User user, Dictionary<String, List<Item>> items)
        {
            searchForPermission();
            foreach (var item in items)
            {
                if (this._itemsInStore.ContainsKey(item.Key))
                {
                    //Already have item with the same name
                    _itemsInStore[item.Key].InsertRange(0,item.Value);
                }
                else
                {
                    this._itemsInStore.Add(item.Key,item.Value);
                }
            }
        }

        public String getStoreName()
        {
            return this._storeName;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public Dictionary<Item,int> searchForStoreItems(String searchString)
        {
            Dictionary<Item, int> itemsFound = new Dictionary<Item, int>();
            foreach (var itemInStore in this._itemsInStore)
            {
                List<Item> itemInven=itemInStore.Value;
                Item first=itemInven[0];
                if (first.checkResemblance(searchString))
                {
                    itemsFound.Add(first, itemInven.Count);
                }
            }

            return itemsFound;
        }

        public Answer<List<Item>> aquireItems(String itemName, int amount)
        {
            if (amount <= 0 || itemName == null || itemName.Length == 0)
            {
                return new Answer<List<Item>>("Bad input");
            }

            if (this._itemsInStore.ContainsKey(itemName))
            {
                if (this._itemsInStore[itemName].Count - amount> 1)
                {
                    List<Item> aquiredItems = new List<Item>();

                    for (int i = 0; i < amount; i++)
                    {
                        aquiredItems.Add(_itemsInStore[itemName][0]);
                        _itemsInStore[itemName].RemoveAt(0);
                    }

                    if (this._aquiredItemsInStore.ContainsKey(itemName))
                    {
                        this._aquiredItemsInStore[itemName].InsertRange(0,aquiredItems);
                    }
                    else
                    {
                        this._aquiredItemsInStore.Add(itemName,aquiredItems);
                    }

                    return new Answer<List<Item>>(aquiredItems);
                }
                else
                {
                    return new Answer<List<Item>>("No enough items in store");
                }
            }
            else
            {
                return new Answer<List<Item>>("Item doesn't exist in store");
            }
        }

        public Answer<List<Item>> purchaseAquiredItems(List<Item> aquiredItems)
        {
            List<Item> unAquiredItems = new List<Item>();
            foreach (var item in aquiredItems)
            {
                if (this._aquiredItemsInStore.ContainsKey(item.getName()) &&
                    this._aquiredItemsInStore[item.getName()].Count==aquiredItems.Count)
                {
                    
                }
            }

            return null;
        }
    }
}