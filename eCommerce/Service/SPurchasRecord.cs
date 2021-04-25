using System;
using eCommerce.Business;

namespace eCommerce.Service
{
    public class SPurchaseRecord
    {
        public string StoreId { get; }
        public SBasket Basket { get; }
        public DateTime PurchaseTime { get; }

        public SPurchaseRecord(string storeId, SBasket basket, DateTime purchaseTime)
        {
            StoreId = storeId;
            Basket = basket;
            PurchaseTime = purchaseTime;
        }
        
        internal SPurchaseRecord(PurchaseRecord purchaseRecord)
        {
            StoreId = purchaseRecord.StoreId;
            Basket = new SBasket(purchaseRecord.BasketInfo);
            PurchaseTime = purchaseRecord.PurchaseTime;
        }
    }
}