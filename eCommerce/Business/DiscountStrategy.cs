using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Business
{
    public interface DiscountStrategy
    {
        public Result<double> GetTotalPrice(IBasket basket);
        public Result<double> GetTotalPrice(IList<ItemInfo> items);
    }
}