using eCommerce.Business.Basics;

namespace eCommerce.Business
{
    public interface PurchaseStrategy
    {
        Answer<Item> getItemsToBasket(int amount);
        PurchaseStrategies getStrategyName();
    }
}