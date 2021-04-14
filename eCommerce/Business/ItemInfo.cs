using System;
using System.Collections.Generic;

namespace eCommerce.Business
{
    public class ItemInfo
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
        
    }
}