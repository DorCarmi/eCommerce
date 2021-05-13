using System.Collections.Generic;
using System.Linq;
using eCommerce.Adapters;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class Transaction
    {
        private ICart _cart;
        private SupplyProxy _supply;
        private PaymentProxy _payment;

        public Transaction(ICart cart)
        {
            this._cart = cart;
            _supply = new SupplyProxy();
            _payment = new PaymentProxy();
        }

        public Result BuyWholeCart(PaymentInfo paymentInfo)
        {
            //Calculate prices for each basket
            foreach (var basket in this._cart.GetBaskets())
            {
                var res=basket.CalculateBasketPrices();
                if (res.IsFailure)
                {
                    return res;
                }
            }
            //Finish buying items
            foreach (var basket in this._cart.GetBaskets())
            {
                var res = basket.BuyWholeBasket();
                if (res.IsFailure)
                {
                    return res;
                }
            }

            double totalPriceForAllBaskets = 0;
            foreach (var basket in this._cart.GetBaskets())
            {
                totalPriceForAllBaskets += basket.GetTotalPrice().GetValue();
            }

            if (totalPriceForAllBaskets <= 0)
            {
                return Result.Fail("Problem with calculating prices: can't charge negative price");
            }

            var payTask=this._payment.Charge(totalPriceForAllBaskets, paymentInfo.UserName, paymentInfo.IdNumber,
                paymentInfo.CreditCardNumber, paymentInfo.CreditCardExpirationDate,
                paymentInfo.ThreeDigitsOnBackOfCard);
            payTask.Wait();
            if (!payTask.Result)
            {
                //Undo get all items
                return Result.Fail("Payment process didn't succeed");
            }

            
            foreach (var basket in this._cart.GetBaskets())
            {
                List<string> itemNames = new List<string>();
                foreach (var item in basket.GetAllItems().GetValue())
                {
                    itemNames.Add(item.name);
                }

                var resSup=this._supply.SupplyProducts(basket.GetStoreName(), itemNames.ToArray(), paymentInfo.FullAddress);
                resSup.Wait();
                if (!resSup.Result)
                {
                    return Result.Fail("Problem supplying from store: " + basket.GetStoreName());
                }
            }

            return Result.Ok();


        }
    }
}