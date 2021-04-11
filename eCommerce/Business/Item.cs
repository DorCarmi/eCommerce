using System;
using System.Collections.Generic;
using eCommerce.Business.Basics;

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

        public Item(String name, Category category, Store store)
        {
            this._name = name;
            this._category = category;
            this._belongsToStore = store;
            _amount = 1;
            this._keyWords=new List<string>();
            _purchaseStrategy = new DefaultPurchasePolicy();
            
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

        public void assignPurchaseStrategy(User user,PurchaseStrategy purchaseStrategy)
        {
            
            this._purchaseStrategy = purchaseStrategy;
        }
    }
}