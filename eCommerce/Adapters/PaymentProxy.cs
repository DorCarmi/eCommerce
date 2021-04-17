using System.Threading.Tasks;
using eCommerce.Business;

namespace eCommerce.Adapters
{
    public class PaymentProxy : IPaymentAdapter
    {
        private IPaymentAdapter _adapter;

        public PaymentProxy()
        {
            _adapter = null;
        }
        
        public async Task<bool> Charge(double price, string paymentInfoUserName, string paymentInfoIDNumber, string paymentInfoCreditCardNumber, string paymentInfoCreditCardExpirationDate, string paymentInfoThreeDigitsOnBackOfCard)
        {
            if (_adapter == null)
            {
                await Task.Delay(100);
                return true;
            }

            return await _adapter.Charge(price,paymentInfoUserName,paymentInfoIDNumber, paymentInfoCreditCardNumber,paymentInfoCreditCardExpirationDate, paymentInfoThreeDigitsOnBackOfCard );
        }
    }
}