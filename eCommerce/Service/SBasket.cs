using System.Collections.Generic;
using eCommerce.Business;
using eCommerce.Business.Service;

namespace eCommerce.Service
{
    public class SBasket
    {
        public string StoreId { get; }
        public IList<IItem> Items { get; }
        public double TotalPrice { get; }

        public SBasket(string storeId, IList<IItem> items, double totalPrice)
        {
            StoreId = storeId;
            Items = items;
            TotalPrice = totalPrice;
        }

        internal SBasket(string storeId, BasketInfo basketInfo)
        {
            StoreId = storeId;
            Items = new List<IItem>(basketInfo.ItemsInBasket);
            TotalPrice = basketInfo.TotalPrice;
        }
    }
}