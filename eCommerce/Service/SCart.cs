using System.Collections;
using System.Collections.Generic;
using eCommerce.Business;
using eCommerce.Business.Service;

namespace eCommerce.Service
{
    public class SCart
    {
        public IList<SBasket> Baskets { get; }
        public string CartHolderID { get; }

        public SCart(IList<SBasket> baskets, string CartHolder)
        {
            Baskets = baskets;
            this.CartHolderID = CartHolder;
        }
        
        internal SCart(IList<IBasket> baskets, string CartHolder)
        {
            this.Baskets = new List<SBasket>();
            this.CartHolderID = CartHolder;
            foreach (var basket in baskets)
            {
                Baskets.Add(new SBasket(basket.GetAllItems().Value as IList<IItem>, basket.GetTotalPrice().Value));
            }
        }
    }
}