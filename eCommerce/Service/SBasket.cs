using System.Collections.Generic;
using eCommerce.Business;
using eCommerce.Business.Service;

namespace eCommerce.Service
{
    public class SBasket
    {
        public IList<IItem> Items { get; }
        public double TotalPrice { get; }

        public SBasket(IList<IItem> items, double totalPrice)
        {
            Items = items;
            TotalPrice = totalPrice;
        }

        internal SBasket(BasketInfo basketInfo)
        {
            Items = new List<IItem>(basketInfo.ItemsInBasket);
            TotalPrice = basketInfo.TotalPrice;
        }
    }
}