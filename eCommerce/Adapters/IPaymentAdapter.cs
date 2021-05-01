using System.Threading.Tasks;
using eCommerce.Business;

namespace eCommerce.Adapters
{
    public interface IPaymentAdapter
    {
        public Task<bool> Charge(double price, string paymentInfoUserName, string paymentInfoIdNumber, string paymentInfoCreditCardNumber, string paymentInfoCreditCardExpirationDate, string paymentInfoThreeDigitsOnBackOfCard);
    }
}