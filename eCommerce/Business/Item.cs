using System;
using System.Collections.Generic;
using eCommerce.Business.Basics;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace eCommerce.Business
{
    public class Item
    {

        private String _name;
        private int _amount;
        private Store _belongsToStore;
        private Category _category;
        private List<String> _keyWords;
        private PurchaseStrategy _purchaseStrategy;
        private int _pricePerUnit;
        private bool _isAquired;
        public int TotalPrice => _amount * _pricePerUnit;

        public Item(String name, Category category, Store store, int pricePerUnit)
        {
            _isAquired = false;
            this._name = name;
            this._category = category;
            this._belongsToStore = store;
            _amount = 1;
            _pricePerUnit = pricePerUnit;
            this._keyWords=new List<string>();
            _purchaseStrategy = new DefaultPurchasePolicy();
        }

        public Item(ItemInfo itemInfo, Store store)
        {
            this._amount = itemInfo.amount;
            this._category = new Category(itemInfo.category);
            this._name = itemInfo.name;
            _keyWords = new List<string>();
            foreach (var word in itemInfo.keyWords)
            {
                if (word == null)
                {
                    throw new ArgumentException("Null key word argument");
                }
                else
                {
                    this._keyWords.Add(word);
                }
            }

            this._belongsToStore = store;
        }

        public Answer<bool> aquireItem()
        {
            if (_isAquired)
            {
                return new Answer<bool>("Item already aquired");
            }
            else
            {
                this._isAquired = true;
                return new Answer<bool>(true);
            }
        }

        public bool IsAquired => _isAquired;

        public int getPricePerUnit()
        {
            return _pricePerUnit;
        }

        public Answer<bool> setPricePerUnit(User user, int newPrice)
        {
            if (user.hasPermission(this._belongsToStore, StorePermission.ChangeItemPrice))
            {
                if (_belongsToStore.checkWithStorePolicy(_pricePerUnit, _amount))
                {
                    return new Answer<bool>(true);
                }
                else
                {
                    return new Answer<bool>("New price doesn't stand with store's policy");
                }
            }
            else
            {
                return new Answer<bool>("User doesn't have the permissions");
            }
        }

        public String getName()
        {
            return this._name;
        }

        public int getAmount()
        {
            return _amount;
        }

        public Answer<int> getItems(int amount)
        {
            if (_amount - amount < 0)
            {
                return new Answer<int>("Bad amount- not enough items");
            }
            else
            {
                this._amount -= amount;
                return new Answer<int>(amount);
            }
        }

        public Answer<bool> addItems(int amount)
        {
            if (amount < 0)
            {
                return new Answer<bool>("Bad amount- less than zero");
            }
            else
            {
                return new Answer<bool>(true);
            }
        }


        public bool checkResemblance(string searchString)
        {
            if (this._name.Contains(searchString))
            {
                return true;
            }
            else if (_category.getName().Contains(searchString))
            {
                return true;
            }
            else if(this._keyWords.Contains(searchString))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public ItemInfo getItemInfo()
        {
            //int amount, string name, string storeName, string category, List<string> keyWords)

            return new ItemInfo(
                this._amount,
                this._name,
                this._belongsToStore.getStoreName(),
                _category.getName(),
                _keyWords
            );
        }

        public Answer<bool> assignPurchaseStrategy(User user,PurchaseStrategies strategyName)
        {
            if (user.hasPermission(_belongsToStore, StorePermission.ChangeItemStrategy))
            {
                if (_belongsToStore.checkWithStorePolicy(strategyName))
                {
                    this._purchaseStrategy = _belongsToStore.getPurchaseStrategy(strategyName);
                    return new Answer<bool>(true);
                }
                else
                {
                    return new Answer<bool>("Purchase strategy not agreed by store policy");
                }
            }
            else
            {
                return new Answer<bool>("Not permission to do that");
            }
        }

        public PurchaseStrategy getCurrentPurchaseStrategy()
        {
            return this._purchaseStrategy;
        }
        
    }
}