using System.Collections.Generic;

namespace eCommerce.Business
{
    public class BasketInfo
    {
        private IList<ItemInfo> _itemsInBasket;
        private readonly double _totalPrice;
        public BasketInfo(IBasket basket)
        {
            var itemsRes = basket.GetAllItems();
            if (!itemsRes.IsFailure)
            {
                foreach (var itemInf in itemsRes.GetValue())
                {
                   _itemsInBasket.Add(itemInf);
                }
            }

            this._totalPrice = basket.GetTotalPrice().GetValue();

        }
    }
}