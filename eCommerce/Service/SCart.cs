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
            Baskets = new List<SBasket>();
            foreach (var basket in baskets)
            {
                var newItems = new List<SItem>();
                var items = basket.GetAllItems().Value;
                foreach (var item in items)
                {
                    newItems.Add(new SItem(item.ItemName, item.StoreName, item.Amount, 
                        item.Category, item.KeyWords, item.PricePerUnit));
                }
                
                Baskets.Add(new SBasket(basket.GetStoreName() , newItems, basket.GetTotalPrice().Value));
            }
        }
    }
}