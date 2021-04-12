using System.Collections.Generic;

namespace eCommerce.Business
{
    public class PurchasePolicy
    {
        private List<PurchaseStrategies> _allowedStrategies;

        public PurchasePolicy()
        {
            this._allowedStrategies = new List<PurchaseStrategies>();
            this._allowedStrategies.Add(PurchaseStrategies.Regular);
        }
        public bool checkStrategy(PurchaseStrategies strategyName)
        {
            return _allowedStrategies.Contains(strategyName);
        }

        public bool checkAmountAndPrice(int pricePerUnit, int amount)
        {
            //TODO: change to something meaningful
            return true;
        }
    }
}