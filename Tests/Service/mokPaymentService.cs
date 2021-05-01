using System.Threading.Tasks;
using eCommerce.Adapters;

namespace Tests.Service
{
    public class mokPaymentService: IPaymentAdapter
    {
        public async Task<bool> Charge(double price, string paymentInfoUserName, string paymentInfoIdNumber, string paymentInfoCreditCardNumber,
            string paymentInfoCreditCardExpirationDate, string paymentInfoThreeDigitsOnBackOfCard)
        {
                await Task.Delay(5000);
                return true;
            
        }
    }
}