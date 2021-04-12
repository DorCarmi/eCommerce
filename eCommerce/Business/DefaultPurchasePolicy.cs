using eCommerce.Business.Basics;

namespace eCommerce.Business
{
    public class DefaultPurchasePolicy: PurchaseStrategy
    {
        public Answer<Item> getItemsToBasket(int amount)
        {
            throw new System.NotImplementedException();
        }

        public PurchaseStrategies getStrategyName()
        {
            throw new System.NotImplementedException();
        }
    }
}