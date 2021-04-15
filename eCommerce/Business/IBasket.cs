using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Business
{
    public interface IBasket
    {
        public Result CalculateBasketPrices();

        Result AddItemToBasket(ItemInfo item);
        Result EditItemInBasket(ItemInfo item);
        Result BuyWholeBasket();
        Result<double>GetTotalPrice();
        Result<IEnumerable<Item>> GetAllItems();
    }
}