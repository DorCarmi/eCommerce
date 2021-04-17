using System;
using System.Collections.Generic;
using System.Net.Sockets;
using eCommerce.Common;

namespace eCommerce.Business
{
    public interface ICart
    {
        public Result AddItemToCart(IUser user,ItemInfo item);
        
        public Result EditCartItem(IUser user,ItemInfo item);
        

        public Result CalculatePricesForCart();

        public Result<PurchaseInfo> BuyWholeCart(IUser user);
        public bool CheckForCartHolder(IUser user);

        public IList<IBasket> GetBaskets();





    }
}