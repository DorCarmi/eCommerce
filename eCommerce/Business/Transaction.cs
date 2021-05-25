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

            var paymentInfoRes = _payment.CheckPaymentInfo(paymentInfo.UserName, paymentInfo.IdNumber,
                paymentInfo.CreditCardNumber, paymentInfo.CreditCardExpirationDate,
                paymentInfo.ThreeDigitsOnBackOfCard);
            paymentInfoRes.Wait();
            if (!paymentInfoRes.Result)
            {
                return Result.Fail("<PaymentInfo>Payment info found incorrect by payment system");
            }
            
            var payTask=this._payment.Charge(totalPriceForAllBaskets, paymentInfo.UserName, paymentInfo.IdNumber,
                paymentInfo.CreditCardNumber, paymentInfo.CreditCardExpirationDate,
                paymentInfo.ThreeDigitsOnBackOfCard);
            payTask.Wait();
            if (!payTask.Result)
            {
                //Undo get all items
                return Result.Fail("<Payment>Payment process didn't succeed");
            }

            foreach (var basket in this._cart.GetBaskets())
            {
                List<string> itemNames = new List<string>();
                foreach (var item in basket.GetAllItems().GetValue())
                {
                    itemNames.Add(item.name);
                }

                var supplyInfoCheck =
                    _supply.CheckSupplyInfo(basket.GetStoreName(), itemNames.ToArray(), paymentInfo.FullAddress);
                supplyInfoCheck.Wait();
                if (!supplyInfoCheck.Result)
                {
                    this._payment.Refund(totalPriceForAllBaskets, paymentInfo.UserName, paymentInfo.IdNumber,
                        paymentInfo.CreditCardNumber, paymentInfo.CreditCardExpirationDate,
                        paymentInfo.ThreeDigitsOnBackOfCard);
                    return Result.Fail("<SupplyInfo>Supply info found incorrect by supply system");
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
                if (!resSup.Result)
                {
                    _payment.Refund(totalPriceForBasket, paymentInfo.UserName, paymentInfo.IdNumber,
                        paymentInfo.CreditCardNumber, paymentInfo.CreditCardExpirationDate,
                        paymentInfo.ThreeDigitsOnBackOfCard);
                    supplyProblems.Add(basket.GetStoreName());
                }
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