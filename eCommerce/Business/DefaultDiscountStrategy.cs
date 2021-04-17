using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class DefaultDiscountStrategy: DiscountStrategy
    {
        public Result<double> GetTotalPrice(IBasket basket)
        {
            if (basket.GetTotalPrice().GetValue() > 500)
            {
                return Result.Ok<double>(basket.GetTotalPrice().GetValue() * 0.9);
            }
            else
            {
                return Result.Ok<double>(basket.GetTotalPrice().GetValue());
            }
            
        }

        public Result<double> GetTotalPrice(IList<ItemInfo> items)
        {
            double totalPrice = 0;
            foreach (var item in items)
            {
                totalPrice += (item.amount * item.pricePerUnit);
            }

            if (totalPrice > 500)
            {
                return Result.Ok(totalPrice * 0.9);
            }
            else
            {
                return Result.Ok(totalPrice);
            }
        }
    }
}