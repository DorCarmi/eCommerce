using System.Collections.Generic;
using eCommerce.Business.CombineRules;
using eCommerce.Business.DiscountPoliciesCombination;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class DefaultDiscount: DiscountComposite
    {
        private List<Composite<double,bool>> rules;

        public DefaultDiscount()
        {
        }

        public void Calculate(IBasket basket)
        {
            
        }

        public bool Check(IBasket basket)
        {
            return true;
        }

        public Result<double> Get(IBasket basket)
        {
            return basket.GetTotalPrice();
        }

        public Result<double> CheckCalculation(IBasket basket)
        {
            return basket.GetTotalPrice();
        }

        public IList<DiscountInfo> GetDiscountInfo(IStore store)
        {
            List<DiscountInfo> discountInfos = new List<DiscountInfo>();
            discountInfos.Add(
                new DiscountInfo(
                    DiscountType.Default,"Anything",ItemInfo.AnyItem(store.GetStoreName()),1 ));
            return discountInfos;
        }
    }
}