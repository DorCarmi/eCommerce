using System.Collections.Generic;
using eCommerce.Business.Discounts;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business.CombineRules
{
    public class DiscountComposite : Composite
    {
        private Composite _rule;
        private double _theDiscount;

        public DiscountComposite(Composite rule,double theDiscount)
        {
            this._rule = rule;
            this._theDiscount = theDiscount;
        }

        public DiscountInfoNode GetDiscountInfo(IStore store)
        {
            return null;
        }

        public Result<double> GetDiscount(IBasket basket, IUser user)
        {
            double newPrice = basket.GetRegularTotalPrice();
            var lst = this._rule.Check(basket,user);
            if (lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    var price = item.Value.amount * item.Value.pricePerUnit;
                    var priceAfterDiscount = item.Value.amount * item.Value.pricePerUnit * _theDiscount;
                    var diff = price - priceAfterDiscount;
                    newPrice -= diff;
                }
            }

            return Result.Ok(newPrice);
        }

        public override Dictionary<string, ItemInfo> Check(IBasket checkItem1, IUser checkItem2)
        {
            return _rule.Check(checkItem1, checkItem2);
        }

        public override bool CheckOneItem(ItemInfo itemInfo, IUser checkItem2)
        {
            throw new System.NotImplementedException();
        }

        public override bool CheckIfDiscount()
        {
            return true;
        }

        public override Result<double> Get(IBasket basket, IUser user)
        {
            return GetDiscount(basket, user);
        }

        public override Result<double> GetOneItem(ItemInfo itemInfo, IUser user)
        {
            return Result.Ok(itemInfo.amount * itemInfo.pricePerUnit * _theDiscount);
        }
    }
}