using eCommerce.Common;

namespace eCommerce.Business.CombineRules
{
    public interface Composite<T,U,G>
    {
        public void Calculate(IBasket basket);

        public U Check(G checkItem);

        
    }
}