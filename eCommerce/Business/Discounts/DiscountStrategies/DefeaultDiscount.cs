using System.Collections.Generic;
using eCommerce.Business.CombineRules;
using eCommerce.Business.DiscountPoliciesCombination;
using eCommerce.Business.Discounts;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class DefaultDiscount: DiscountComposite
    {
        public DefaultDiscount()
        {
        }

        public void Calculate(IBasket basket, IUser user)
        {
            
        }

        public bool Check(IBasket basket, IUser user)
        {
            return true;
        }

        public Result<double> Get(IBasket basket, IUser user)
        {
            return basket.GetTotalPrice();
        }

        public Result<double> CheckCalculation(IBasket basket, IUser user)
        {
            return basket.GetTotalPrice();
        }

        public CombinationDiscountInfoNode GetDiscountInfo(IStore store)
        {
            List<DiscountInfo> discountInfos = new List<DiscountInfo>();
            discountInfos.Add(
                new DiscountInfo(
                    DiscountType.Default,"Anything",ItemInfo.AnyItem(store.GetStoreName()),1 ));
            return new CombinationDiscountInfoLeaf(discountInfos);
        }
    }
}