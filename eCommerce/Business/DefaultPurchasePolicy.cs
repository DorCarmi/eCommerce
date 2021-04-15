using eCommerce.Business.Basics;

namespace eCommerce.Business
{
    public class DefaultPurchasePolicy : PurchasePolicy
    {
        /// <summary>
        /// <Use case> </Use Case>
        /// <Test></Test>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
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