using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using eCommerce.Common;
using eCommerce.Service;

namespace eCommerce.Business
{
    public class ItemInfo : IItem
    {
        public int amount;
        public string name;
        public string storeName;
        public string category;
        public List<string> keyWords;
        public double pricePerUnit;
        private Item theItem;
        private double discountFactor;
        private Store _store;

        public static ItemInfo AnyItem(string storeName) =>
            new ItemInfo(0, "ANY", storeName, "ALL", new List<string>(), 0);
        public ItemInfo(int amount, string name, string storeName, string category,
            double pricePerUnit, List<string> keyWords,Item theItem)
        {
            this.amount = amount;
            this.name = name;
            this.storeName = storeName;
            this.category = category;
            this.keyWords = new List<string>();
            this.pricePerUnit = pricePerUnit;
            foreach (var word in keyWords)
            {
                if (word == null)
                {
                    throw new ArgumentException("Bad key word- null");
                }
                else
                {
                    this.keyWords.Add(String.Copy(word));
                }
                
            }

            this.theItem = theItem;
        }
        
        public ItemInfo(int amount, string name, string storeName, 
            string category, List<string> keyWords, double pricePerUnit)
        {
            this.amount = amount;
            this.name = name;
            this.storeName = storeName;
            this.category = category;
            this.keyWords = new List<string>();
            this.pricePerUnit = pricePerUnit;
            foreach (var word in keyWords)
            {
                if (word == null)
                {
                    throw new ArgumentException("Bad key word- null");
                }
                else
                {
                    this.keyWords.Add(String.Copy(word));
                }
                
            }
            this.theItem = null;
        }

        public ItemInfo(ItemInfo itemInf)
        {
            this.amount = itemInf.amount;
            this.category = itemInf.category;
            this.name = itemInf.name;
            this.discountFactor = itemInf.discountFactor;
            this.keyWords = new List<string>();
            foreach (var keyWord in itemInf.keyWords)
            {
                this.keyWords.Add(keyWord);
            }

            this.storeName = itemInf.storeName;
            this.theItem = itemInf.theItem;
            this.pricePerUnit = itemInf.pricePerUnit;
        }

        public Result SetItemToStore(Store store)
        {
            if (store != null)
            {
                this._store = store;
                return Result.Ok();
            }
            else
            {
                return Result.Fail("Problem assigning store");
            }
        }
        

        public Result<Store> GetStore()
        {
            if (this._store == null)
            {
                return Result.Fail<Store>("No store assigned to item");
            }
            else
            {
                return Result.Ok(this._store);
            }
            
        }

        public Result AssignStoreToItem(Store store)
        {
            if (store.GetStoreName().Equals(this.storeName))
            {
                this._store = store;
                return Result.Ok();
            }
            else
            {
                return Result.Fail("Store assigned doesn't match item's store name");
            }
        }

        public void ApplyUniqueDiscountOnProduct(double discountFactor)
        {
            this.discountFactor = discountFactor; 
        }

        // ========== Properties ========== //
        
        // TODO check how to set store and other properties
        // that dont have setters
        
        public string ItemName { get => name; }
        public string StoreName { get => storeName; }

        public int Amount
        {
            get => amount;
            set => amount = value;
        }

        public string Category
        {
            get => category;
            set => category = value;
        }

        public List<string> KeyWords
        {
            get => keyWords;
            set => keyWords = value;
        }
        public double PricePerUnit
        {
            get => pricePerUnit;
            set => pricePerUnit = value;
        }
    }
}