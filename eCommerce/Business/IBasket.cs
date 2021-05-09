using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Business
{
    public interface IBasket
    {
        public Result CalculateBasketPrices();

        public Result AddItemToBasket(IUser user,ItemInfo item);
        public Result EditItemInBasket(IUser user,ItemInfo item);
        public Result BuyWholeBasket();
        public Result<double>GetTotalPrice();
        public Result<IList<ItemInfo>> GetAllItems();
        public ICart GetCart();
        Result SetTotalPrice();
        public void SetTotalPrice(double newTotalPrice);
        public string GetStoreName();
        public Result<ItemInfo> GetItem(IUser user, string itemName);
        public BasketInfo GetBasketInfo();
    }
}