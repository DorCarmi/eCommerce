using eCommerce.Business;


namespace Tests.Business.Mokups
{
    public class MokBasket : Basket
    {
        public MokBasket(ICart cart, Store store) : base(cart, store)
        {
        }
    }
}