﻿using System;
using System.Collections.Generic;


namespace eCommerce.Business
{
    public class BasketInfo
    {
        public IList<ItemInfo> _itemsInBasket;
        public readonly double _totalPrice;
        public String storeName;
        public BasketInfo(IBasket basket)
        {
            _itemsInBasket = new List<ItemInfo>();
            var itemsRes = basket.GetAllItems();
            if (!itemsRes.IsFailure)
            {
                foreach (var itemInf in itemsRes.GetValue())
                {
                   _itemsInBasket.Add(new ItemInfo(itemInf));
                }
            }

            this._totalPrice = basket.GetTotalPrice().GetValue();
            this.storeName = basket.GetStoreName();
        }

        public IList<ItemInfo> ItemsInBasket
        {
            get => _itemsInBasket;
        }

        public double TotalPrice => _totalPrice;
    }
}