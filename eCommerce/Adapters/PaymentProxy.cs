using System.Threading.Tasks;
using eCommerce.Business;

namespace eCommerce.Adapters
{
    public class PaymentProxy : IPaymentAdapter
    {
        private static IPaymentAdapter _adapter;
        public static int PROXY_REFUNDS=0;
        public static int REAL_REFUNDS=0;
        public static int REAL_HITS = 0;
        public static int PROXY_HITS = 0;

        public PaymentProxy()
        {
        }

        public static void AssignPaymentService(IPaymentAdapter paymentAdapter)
        {
            _adapter = paymentAdapter;
        }
        
        public async Task<bool> Charge(double price, string paymentInfoUserName, string paymentInfoIDNumber, string paymentInfoCreditCardNumber, string paymentInfoCreditCardExpirationDate, string paymentInfoThreeDigitsOnBackOfCard)
        {
            if (_adapter == null)
            {
                await Task.Delay(5000);
                PROXY_HITS++;
                return true;
            }
            var ans=await _adapter.Charge(price,paymentInfoUserName,paymentInfoIDNumber, paymentInfoCreditCardNumber,paymentInfoCreditCardExpirationDate, paymentInfoThreeDigitsOnBackOfCard );
            if (ans)
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

        public async Task<bool> Refund(double price, string paymentInfoUserName, string paymentInfoIdNumber, string paymentInfoCreditCardNumber,
            string paymentInfoCreditCardExpirationDate, string paymentInfoThreeDigitsOnBackOfCard)
        {
            if (_adapter == null)
            {
                await Task.Delay(5000);
                PROXY_REFUNDS++;
                return true;
            }

            
            var ans=await _adapter.Refund(price,paymentInfoUserName,paymentInfoIdNumber, paymentInfoCreditCardNumber,paymentInfoCreditCardExpirationDate, paymentInfoThreeDigitsOnBackOfCard );
            if (ans)
            {
                REAL_REFUNDS++;
            }

            return ans;
        }
    }
}