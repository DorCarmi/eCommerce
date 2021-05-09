﻿using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class Dates : DiscountComposite
    {
        private Dictionary<DateTime,double> _datesDiscounts;
        public Dates()
        {
            this._datesDiscounts = new Dictionary<DateTime,double>();
        }

        public void AddDateAndDiscount(DateTime date, double discount)
        {
            this._datesDiscounts.Add(date,discount);
        }

        public void Calculate(IBasket basket, IUser user)
        {
            var res = CheckCalculation(basket,user);
            if (res.IsSuccess)
            {
                basket.SetTotalPrice(res.Value);
            }
        }

        public bool Check(IBasket basket, IUser user)
        {
            foreach (var datesDiscount in _datesDiscounts)
            {
                if (datesDiscount.Key.Date.Equals(DateTime.Now.Date))
                {
                    return true;
                }
            }

            return false;
        }

        public Result<double> Get(IBasket basket, IUser user)
        {
            return basket.GetTotalPrice();
        }

        public Result<double> CheckCalculation(IBasket basket, IUser user)
        {
            var totalPrice = basket.GetTotalPrice();
            if (totalPrice.IsFailure)
            {
                return totalPrice;
            }

            var minDiscountValue = totalPrice.Value;
            foreach (var datesDiscount in _datesDiscounts)
            {
                if (datesDiscount.Key.Date.Equals(DateTime.Now.Date))
                {
                    var curr = totalPrice.Value * datesDiscount.Value;
                    if (curr < minDiscountValue)
                    {
                        minDiscountValue = curr;
                    }
                }
            }
            return Result.Ok(minDiscountValue);
        }

        public CombinationDiscountInfoNode GetDiscountInfo(IStore store)
        {
            List<DiscountInfo> discountInfos = new List<DiscountInfo>();
            foreach (var item in this._datesDiscounts)    
            {
                
                discountInfos.Add(
                    new DiscountInfo(
                        DiscountType.Category,"Date = "+item.Key.Date,
                        ItemInfo.AnyItem(store.GetStoreName()), item.Value));
            }

            return new CombinationDiscountInfoLeaf(discountInfos);
        }
    }
}