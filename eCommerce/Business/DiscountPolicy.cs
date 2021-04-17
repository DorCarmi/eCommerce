using System.Collections.Generic;
using System.Linq;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class DiscountPolicy
    {
        private IList<DiscountRule> _allowedDiscounts;
        private IStore _store;

        public DiscountPolicy(IStore store)
        {
            this._store = store;
            this._allowedDiscounts = new List<DiscountRule>();
            this._allowedDiscounts.Add(new DiscountRule());
        }
        public bool checkDiscount(DiscountRule discountRule)
        {
            return _allowedDiscounts.FirstOrDefault(x => x.CheckIfOK(discountRule)) != null;
        }

        public bool CheckBasket(Basket basket)
        {
            //TODO: change to something meaningful
            return true;
        }

        public Result AddAllowedDiscountRule(User user)
        {
            if (user.hasPermission(_store, StorePermission.EditStorePolicy))
            {
                return Result.Ok();
            }
            else
            {
                return Result.Fail("User doesn't have permission to edit store's policy");
            }
        }
    }
}