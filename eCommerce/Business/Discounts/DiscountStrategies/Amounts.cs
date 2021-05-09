using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class Amounts: DiscountComposite
    {
        private Dictionary<ItemInfo,Discount> _itemsAndAmounts;
        private List<Discount> _totalAmounts;


        public Amounts()
        {
            this._itemsAndAmounts = new Dictionary<ItemInfo, Discount>();
            _totalAmounts = new List<Discount>();
            //_totalAmounts.Add(new Discount(0, 1));
        }

        public void AddItemAmountDiscount(ItemInfo item, int amount, double discount)
        {
            this._itemsAndAmounts.Add(item,new Discount(amount,discount));
        }

        public void AddTotalAmount(int amount, double discount)
        {
            this._totalAmounts.Add(new Discount(amount,discount));
        }
        

        public void Calculate(IBasket basket, IUser user)
        {
            if (basket != null)
            {
                var res = CheckCalculation(basket,user);
                if (res.IsSuccess)
                {
                    basket.SetTotalPrice(res.Value);
                }
            }

        }

        private bool CheckForSpecificItemDiscount(IBasket basket)
        {
            bool ans = false;
            foreach (var item in basket.GetAllItems().Value)
            {
                foreach (var itemsAndAmount in this._itemsAndAmounts) 
                {
                    if (itemsAndAmount.Key.name.Equals(item.name) &&
                        itemsAndAmount.Key.storeName.Equals(item.storeName))
                    {
                        ans = true;
                        break;
                    }
                }
            }

            return ans;
        }
        
        private bool CheckForTotalPriceDiscount(IBasket basket)
        {
            bool ans = false;
            Discount basicPrice = new Discount(0, 1);
            int totalAmount = 0;
            foreach (var itemInfo in basket.GetAllItems().Value)
            {
                totalAmount += itemInfo.amount;
            }
            foreach (var amountDiscount in _totalAmounts)
            {
                if (amountDiscount.GetAmount() <= totalAmount)
                {
                    ans = true;
                    break;
                }
            }

            return ans;
        }

        public bool Check(IBasket basket, IUser user)
        {
            return CheckForSpecificItemDiscount(basket) || 
                   CheckForTotalPriceDiscount(basket);
        }

        public Result<double> Get(IBasket basket, IUser user)
        {
            Calculate(basket,user);
            return basket.GetTotalPrice();
        }


        public Result<double> CheckCalculation(IBasket basket, IUser user)
        {
            int totalAmount = 0;
            double totalPrice = 0;
            foreach (var item in basket.GetAllItems().Value)
            {
                totalAmount += item.amount;
                double currPrice = 0;
                foreach (var itemsAndAmount in this._itemsAndAmounts) 
                {
                    if (itemsAndAmount.Key.name.Equals(item.name) &&
                        itemsAndAmount.Key.storeName.Equals(item.storeName))
                    {
                        currPrice = itemsAndAmount.Value.CheckDiscount(item.amount, item.pricePerUnit);
                        break;
                    }
                }
                totalPrice += currPrice;
            }

            var currTotalPrice = totalPrice;
            
            foreach (var amountDiscount in _totalAmounts)
            {
                double currPrice = amountDiscount.CalculateAmountForTotalPrice(totalAmount, totalPrice);
                if (currPrice < currTotalPrice)
                {
                    currTotalPrice = currPrice;
                }
            }

            return Result.Ok(currTotalPrice);
        }


        public CombinationDiscountInfoNode GetDiscountInfo(IStore store)
        {
            List<DiscountInfo> discountInfos = new List<DiscountInfo>();
            foreach (var itemsAndAmount in this._itemsAndAmounts)    
            {
                
                discountInfos.Add(
                    new DiscountInfo(
                        DiscountType.Amount,"Amount >= "+itemsAndAmount.Value.GetAmount(),
                        itemsAndAmount.Key,itemsAndAmount.Value.GetDiscount()));
            }

            foreach (var totalAmount in this._totalAmounts)
            {
                discountInfos.Add(
                    new DiscountInfo(
                        DiscountType.Amount,"Total amount >= "+totalAmount.GetAmount(),
                        ItemInfo.AnyItem(store.GetStoreName()),totalAmount.GetDiscount() ));
            }

            return new CombinationDiscountInfoLeaf(discountInfos);
        }
    }
}