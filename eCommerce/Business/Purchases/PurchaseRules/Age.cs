using eCommerce.Common;

namespace eCommerce.Business.PurchaseRules
{
    public class Age : CompositeRule
    {
        public void Calculate(IBasket basket)
        {
            return;
        }

        public bool Check(IBasket basket)
        {
            basket.
        }

        public Result<bool> Get(IBasket basket)
        {
            throw new System.NotImplementedException();
        }

        public Result<bool> CheckCalculation(IBasket basket)
        {
            throw new System.NotImplementedException();
        }
    }
}