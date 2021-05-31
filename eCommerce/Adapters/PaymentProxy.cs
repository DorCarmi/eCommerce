﻿using System.Threading.Tasks;
using eCommerce.Business;
using eCommerce.Common;

namespace eCommerce.Adapters
{
    public class PaymentProxy : IPaymentAdapter
    {
        private static IPaymentAdapter _adapter;
        public static int PROXY_REFUNDS=0;
        public static int REAL_REFUNDS=0;
        public static int REAL_HITS = 0;
        public static int PROXY_HITS = 0;

        private int _transactionId;
        public PaymentProxy()
        {
            _transactionId = 10000;
            _adapter = new WSEPPaymentAdapter();
        }

        public static void AssignPaymentService(IPaymentAdapter paymentAdapter)
        {
            _adapter = paymentAdapter;
        }
        
        public async Task<Result<int>> Charge(double price, string paymentInfoUserName, string paymentInfoIDNumber, string paymentInfoCreditCardNumber, string paymentInfoCreditCardExpirationDate, string paymentInfoThreeDigitsOnBackOfCard)
        {
            if (_adapter == null)
            {
                int transactionId = _transactionId;
                transactionId++;
                await Task.Delay(5000);
                PROXY_HITS++;
                return Result.Ok(transactionId);
            }
            var ans = await _adapter.Charge(price,paymentInfoUserName,paymentInfoIDNumber, paymentInfoCreditCardNumber,paymentInfoCreditCardExpirationDate, paymentInfoThreeDigitsOnBackOfCard );
            if (ans.IsSuccess)
            {
                REAL_HITS++;
            }

            return ans;
        }

        public async Task<bool> CheckPaymentInfo(string paymentInfoUserName, string paymentInfoIDNumber, string paymentInfoCreditCardNumber,
            string paymentInfoCreditCardExpirationDate, string paymentInfoThreeDigitsOnBackOfCard)
        {
            if (_adapter == null)
            {
                await Task.Delay(5000);
                return true;
            }
            var ans=await _adapter.CheckPaymentInfo(paymentInfoUserName,paymentInfoIDNumber, paymentInfoCreditCardNumber,paymentInfoCreditCardExpirationDate, paymentInfoThreeDigitsOnBackOfCard );
            return ans;
        }

        public async Task<Result> Refund(int transactionId)
        {
            if (_adapter == null)
            {
                await Task.Delay(5000);
                PROXY_REFUNDS++;
                return Result.Ok();
            }

            
            var ans = await _adapter.Refund(transactionId);
            if (ans.IsSuccess)
            {
                REAL_REFUNDS++;
            }

            return ans;
        }
    }
}