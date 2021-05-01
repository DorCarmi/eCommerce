using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class Categories: DiscountStrategy
    {
        public Result<double> GetTotalPrice(IBasket basket)
        {
            throw new System.NotImplementedException();
        }

        public Result<double> GetTotalPrice(IList<ItemInfo> items)
        {
            throw new System.NotImplementedException();
        }
    }
}