using System.Collections.Generic;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class Categories: DiscountComposite
    {
        private Dictionary<string, double> CategoryNameToDiscount;
        public Categories(IBasket basket)
        {
            this.CategoryNameToDiscount = new Dictionary<string, double>();
        }

        public void AddCategoryDiscount(string category, double discount)
        {
            if (CategoryNameToDiscount.ContainsKey(category))
            {
                this.CategoryNameToDiscount[category] = discount;
            }
            else
            {
                CategoryNameToDiscount.Add(category,discount);
            }
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
            bool ans = false;
            foreach (var item in basket.GetAllItems().Value)
            {
                if(this.CategoryNameToDiscount.ContainsKey(item.category))
                {
                    ans = true;
                    break;
                }
            }

            return ans;
        }

        public Result<double> Get(IBasket basket, IUser user)
        {
            Calculate(basket,user);
            return basket.GetTotalPrice();
        }

        public Result<double> CheckCalculation(IBasket basket, IUser user)
        {
            double totalPrice = 0;
            foreach (var itemInfo in basket.GetAllItems().Value)
            {
                if (this.CategoryNameToDiscount.ContainsKey(itemInfo.category))
                {
                    totalPrice += itemInfo.amount * itemInfo.pricePerUnit * CategoryNameToDiscount[itemInfo.category];
                }
                else
                {
                    totalPrice += itemInfo.amount * itemInfo.pricePerUnit;
                }
            }

            return Result.Ok(totalPrice);
        }

        public CombinationDiscountInfoNode GetDiscountInfo(IStore store)
        {
            List<DiscountInfo> discountInfos = new List<DiscountInfo>();
            foreach (var item in this.CategoryNameToDiscount)    
            {
                
                discountInfos.Add(
                    new DiscountInfo(
                        DiscountType.Category,"Category = "+item.Key,
                        ItemInfo.AnyItem(store.GetStoreName()), item.Value));
            }

            return new CombinationDiscountInfoLeaf(discountInfos);
        }
    }
}