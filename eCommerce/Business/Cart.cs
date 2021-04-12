using System.Collections.Generic;
using System.Transactions;
using eCommerce.Business.Basics;

namespace eCommerce.Business
{
    public class Cart
    {
        private User _cartHolder;
        private Transaction _performTransaction;
        
        private Dictionary<Store, Basket> _baskets;

        public Cart(User user)
        {
            this._cartHolder = user;
            _baskets = new Dictionary<Store, Basket>();
        }

        public Answer<bool> addItemToCart(Store store, Item item)
        {
            if (_baskets.ContainsKey(store))
            {
                //_baskets[store].addItemToBasket();
                
            }

            return null;
        }
        


    }
}