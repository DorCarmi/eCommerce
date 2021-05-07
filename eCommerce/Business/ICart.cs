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
        

        public Result<double> CalculatePricesForCart();

        public Result BuyWholeCart(IUser user, PaymentInfo paymentInfo);
        public bool CheckForCartHolder(IUser user);

        public IList<IBasket> GetBaskets();


        public CartInfo GetCartInfo();

        public IUser GetUser();
    }
}