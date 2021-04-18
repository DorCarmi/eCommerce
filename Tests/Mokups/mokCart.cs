using System.Collections.Generic;
using eCommerce;
using eCommerce.Business;
using eCommerce.Common;

namespace Tests.Mokups
{
    public class mokCart : ICart
    {
        public Result AddItemToCart(IUser user, ItemInfo item)
        {
            throw new System.NotImplementedException();
        }

        public Result EditCartItem(IUser user, ItemInfo item)
        {
            throw new System.NotImplementedException();
        }

        public Result<double> CalculatePricesForCart()
        {
            throw new System.NotImplementedException();
        }

        public Result BuyWholeCart(IUser user, PaymentInfo paymentInfo)
        {
            throw new System.NotImplementedException();
        }

        public Result<PurchaseInfo> BuyWholeCart(IUser user)
        {
            throw new System.NotImplementedException();
        }

        public bool CheckForCartHolder(IUser user)
        {
            throw new System.NotImplementedException();
        }

        public IList<IBasket> GetBaskets()
        {
            throw new System.NotImplementedException();
        }
    }
}