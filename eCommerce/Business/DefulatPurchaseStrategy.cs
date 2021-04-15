using eCommerce.Common;

namespace eCommerce.Business
{
    public class DefaultPurchaseStrategy : PurchaseStrategy
    {

        public static Result<PurchaseStrategy> GetPurchaseStrategyByName(PurchaseStrategyName purchaseStrategyName)
        {
            if (PurchaseStrategyName.Regular == purchaseStrategyName)
            {
                return Result.Ok<PurchaseStrategy>(new  DefaultPurchaseStrategy());
            }
            else
            {
                return Result.Fail<PurchaseStrategy>("No such strategy");
            }
        }
        
    }
    
}