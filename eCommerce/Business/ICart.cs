using System;
using System.Collections.Generic;
using System.Net.Sockets;
using eCommerce.Common;

namespace eCommerce.Business
{
    public interface ICart
    {
        public Result AddItemToCart(User user,ItemInfo item);
        
        public Result EditCartItem(User user,ItemInfo item);
        

        public Result CalculatePricesForCart();

        public Result<PurchaseInfo> BuyWholeCart(User user);

        



    }
}