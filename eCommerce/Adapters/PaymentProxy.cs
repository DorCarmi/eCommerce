using System.Threading.Tasks;
using eCommerce.Business;

namespace eCommerce.Adapters
{
    public class PaymentProxy : IPaymentAdapter
    {
        private static IPaymentAdapter _adapter;

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
                return true;
            }

            return await _adapter.Charge(price,paymentInfoUserName,paymentInfoIDNumber, paymentInfoCreditCardNumber,paymentInfoCreditCardExpirationDate, paymentInfoThreeDigitsOnBackOfCard );
        }
    }
}