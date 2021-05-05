using System.Collections.Generic;
using eCommerce.Business.Discounts;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business.CombineRules
{
    public interface DiscountComposite : Composite<IBasket, IUser>
    {
        public void Calculate(IBasket basket, IUser user);
        public CombinationDiscountInfoNode GetDiscountInfo(IStore store);
        
        public Result<double> Get(IBasket basket,IUser user);
        public Result<double> CheckCalculation(IBasket basket,IUser user);
    }
}