using System.Collections.Generic;
using System.Net.Sockets;
using eCommerce.Common;

namespace eCommerce.Business
{
    public interface ICart
    {
        public Result AddItemToCart(Item item);

        public Result CalculatePricesForCart();

        public Result<CartInfo> ShowCart();

        public Result BuyWholeCart();

        



    }
}