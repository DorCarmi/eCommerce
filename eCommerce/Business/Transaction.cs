using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            int paymentTransactionId;
            int supplyTransactionId;
            //Check with store policy
            foreach (var basket in this._cart.GetBaskets())
            {
                var res = basket.CheckWithStorePolicy();
                if (res.IsFailure)
                {
                    return res;
                }
            }
            
            //Calculate prices for each basket
            foreach (var basket in this._cart.GetBaskets())
            {
                var res=basket.CalculateBasketPrices();
                if (res.IsFailure)
                {
                    return Result.Fail("<CalculateBaskets>"+res.Error);
                }
            }
            //Finish buying items
            foreach (var basket in this._cart.GetBaskets())
            {
                var res = basket.BuyWholeBasket();
                if (res.IsFailure)
                {
                    return Result.Fail("<GetBaskets>"+res.Error);
                }
            }

            double totalPriceForAllBaskets = 0;
            foreach (var basket in this._cart.GetBaskets())
            {
                totalPriceForAllBaskets += basket.GetTotalPrice().GetValue();
            }

            if (totalPriceForAllBaskets <= 0)
            {
                return Result.Fail("<TotalPrice>Problem with calculating prices: can't charge negative price");
            }

            var payTask=this._payment.Charge(totalPriceForAllBaskets, paymentInfo.UserName, paymentInfo.IdNumber,
                paymentInfo.CreditCardNumber, paymentInfo.CreditCardExpirationDate,
                paymentInfo.ThreeDigitsOnBackOfCard);
            payTask.Wait();
            if (payTask.Result.IsFailure)
            {
                //Undo get all items
                return Result.Fail(payTask.Result.Error);
            }
            
            paymentTransactionId = payTask.Result.Value;
            
            foreach (var basket in this._cart.GetBaskets())
            {
                List<string> itemNames = new List<string>();
                foreach (var item in basket.GetAllItems().GetValue())
                {
                    itemNames.Add(item.name);
                }
            }

            List<string> supplyProblems = new List<string>();
            foreach (var basket in this._cart.GetBaskets())
            {
                List<string> itemNames = new List<string>();
                double totalPriceForBasket = basket.GetTotalPrice().Value;
                foreach (var item in basket.GetAllItems().GetValue())
                {
                    itemNames.Add(item.name);
                }

                
                var resSup=this._supply.SupplyProducts(basket.GetStoreName(), itemNames.ToArray(), paymentInfo.FullAddress);
                resSup.Wait();
                if (resSup.Result.IsFailure)
                {
                    _payment.Refund(paymentTransactionId);
                    supplyProblems.Add(basket.GetStoreName());
                }

                supplyTransactionId = resSup.Result.Value;
            }

            foreach (var basket in _cart.GetBaskets())
            {
                if (supplyProblems.FirstOrDefault(x => x.Equals(basket.GetStoreName())) == null)
                {
                    basket.AddBasketRecords();
                }
            }

            if (supplyProblems.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<Supply>Some of the stores had problems with supply. Stores are: ");
                foreach (var supplyProblem in supplyProblems)
                {
                    stringBuilder.Append(supplyProblem + ", ");
                }

                stringBuilder.Append(".");
                return Result.Fail(stringBuilder.ToString());
            }

            return Result.Ok();
        }
    }
}