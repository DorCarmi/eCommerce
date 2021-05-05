using System.Collections.Generic;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business.CombineRules
{
    public interface DiscountComposite : Composite<double, bool, IBasket>
    {
        public IList<DiscountInfo> GetDiscountInfo(IStore store);
        
        public Result<double> Get(IBasket basket);
        public Result<double> CheckCalculation(IBasket basket);
    }
}