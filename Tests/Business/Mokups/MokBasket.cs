using eCommerce.Business;
using eCommerce.Business.Service;

namespace Tests.Business.Mokups
{
    public class MokBasket : Basket
    {
        public MokBasket(ICart cart, IStore store) : base(cart, store)
        {
        }
    }
}