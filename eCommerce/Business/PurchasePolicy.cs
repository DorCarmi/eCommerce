using System.Collections.Generic;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class PurchasePolicy
    {
        private List<PurchaseStrategyName> _allowedStrategies;
        private IStore _store;

        public PurchasePolicy(IStore store)
        {
            this._store = store;
            this._allowedStrategies = new List<PurchaseStrategyName>();
            this._allowedStrategies.Add(PurchaseStrategyName.Regular);
        }
        public bool checkStrategy(PurchaseStrategyName strategyName)
        {
            return _allowedStrategies.Contains(strategyName);
        }

        public bool checkAmountAndPrice(int pricePerUnit, int amount)
        {
            //TODO: change to something meaningful
            return true;
        }

        public bool CheckBasket(Basket basket)
        {
            //TODO: change to something meaningful
            return true;
        }

        public Result AddAllowedPurchaseStrategy(User user)
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

        public Result AddTimesToStore(User user)
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

        public Result AddPriceRule(User user)
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