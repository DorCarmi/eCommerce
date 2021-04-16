using System.Collections.Generic;

namespace eCommerce.Business.Service
{
    public class PurchaseHistory
    {
        private IList<IBasket> _basketes;
        public IList<IBasket> Baskets
        {
            get => _basketes;
        }

        public PurchaseHistory(IList<IBasket> basketes)
        {
            _basketes = basketes;
        }
    }
}