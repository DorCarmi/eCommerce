using System.Collections;
using System.Collections.Generic;
using eCommerce.Business;
using eCommerce.Business.Service;

namespace eCommerce.Service
{
    public class SCart
    {
        public IList<SBasket> Baskets { get; }

        public SCart(IList<SBasket> baskets)
        {
            Baskets = baskets;
        }
        
        internal SCart(IList<IBasket> baskets)
        {
            IList<SBasket> SBaskets = new List<SBasket>();
            foreach (var basket in baskets)
            {
                SBaskets.Add(new SBasket(basket.GetStoreName() ,basket.GetAllItems().Value as IList<IItem>, basket.GetTotalPrice().Value));
            }
        }
    }
}