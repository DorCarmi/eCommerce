﻿using System;
using System.Collections.Generic;
using eCommerce.Business.Service;

namespace eCommerce.Business
{
    public class ItemInfo
    {
        public int amount;
        public string name;
        public string storeName;
        public string category;
        public List<string> keyWords;
        public int pricePerUnit;
        private Item theItem;
        private double discountFactor;

        public ItemInfo(int amount, string name, string storeName, string category,int pricePerUnit, List<string> keyWords,Item theItem)
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
        
        public ItemInfo(int amount, string name, string storeName, string category, List<string> keyWords, int pricePerUnit)
        {
            this.amount = amount;
            this.name = name;
            this.storeName = storeName;
            this.category = category;
            this.pricePerUnit = pricePerUnit;
            this.keyWords = new List<string>();
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

        public IStore GetStore()
        {
            return theItem.GetStore();
        }

        public void ApplyUniqueDiscountOnProduct(double discountFactor)
        {
            this.discountFactor = discountFactor; 
        }
        
        
        
    }
}