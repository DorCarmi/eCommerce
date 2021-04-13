using eCommerce.Common;

namespace eCommerce.Business
{
    public interface IBasket
    {
        public Result CalculateBasketPrices();
        
    }
}