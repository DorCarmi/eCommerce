using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using eCommerce.Business.Service;

namespace eCommerce.Business
{
    public class ItemInfo : IProduct
    {
        public int amount;
        public string name;
        public string storeName;
        public string category;
        public IList<string> keyWords;
        public int pricePerUnit;

        public ItemInfo(int amount, string name, string storeName, string category, IList<string> keyWords)
        {
            this.amount = amount;
            this.name = name;
            this.storeName = storeName;
            this.category = category;
            keyWords = new List<string>();
            foreach (var word in keyWords)
            {
                if (word == null)
                {
                    throw new ArgumentException("Bad key word- null");
                }
                else
                {
                    keyWords.Add(String.Copy(word));
                }
                
            }
        }

        public string ProductName { get => name; }
        public string StoreName { get => storeName; }
        public int Amount { get => amount; }
        public string Category { get => category; }
        public ReadOnlyCollection<string> KeyWords { get => new ReadOnlyCollection<string>(keyWords); }
        public float PricePerUnit { get => pricePerUnit; }
    }
}