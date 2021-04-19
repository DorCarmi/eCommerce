using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Business.Service
{
    public class BasketDto
    {
        public string StoreId { get; }
        public IList<ItemInfo> Items { get; }
        public double TotalPrice { get; }

        public BasketDto(string storeId, IList<ItemInfo> items, double totalPrice)
        {
            StoreId = storeId;
            Items = items;
            TotalPrice = totalPrice;
        }
    }
}