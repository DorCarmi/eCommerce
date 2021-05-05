using System.Collections.Generic;
using System.Linq;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class DiscountPolicy
    {
        private IList<DiscountComposite> _storeDiscounts;
        private IStore _store;

        public DiscountPolicy(IStore store)
        {
            this._store = store;
            this._storeDiscounts = new List<DiscountComposite>();
            this._storeDiscounts.Add(new DefaultDiscount());
        }

        public Result AddDiscountToStore(DiscountComposite discount)
        {
            this._storeDiscounts.Add(discount);
            return Result.Ok();
        }

        public Result RemoveDiscount(DiscountInfo discountInfo)
        {
            foreach (var storeDiscount in _storeDiscounts)
            {
                if (discountInfo.Equals(storeDiscount.GetDiscountInfo(_store)))
                {
                    _storeDiscounts.Remove(storeDiscount);
                    return Result.Ok();
                }
            }

            return Result.Fail("Discount not found");
        }
    }
}