using System.Threading.Tasks;
using eCommerce.Adapters;

namespace Tests.Service
{
    public class mokPaymentService: IPaymentAdapter
    {
        private bool chargeAns;
        private bool checkAns;
        private bool refundAns;
        public mokPaymentService(bool chargeAnswer, bool checkAnswer, bool refundAns)
        {
            this.chargeAns = chargeAnswer;
            this.checkAns = checkAnswer;
            this.refundAns = refundAns;
        }
        public async Task<bool> Charge(double price, string paymentInfoUserName, string paymentInfoIdNumber, string paymentInfoCreditCardNumber,
            string paymentInfoCreditCardExpirationDate, string paymentInfoThreeDigitsOnBackOfCard)
        {
                await Task.Delay(5000);
                return chargeAns;
            
        }

        public async Task<bool> CheckPaymentInfo(string paymentInfoUserName, string paymentInfoIdNumber, string paymentInfoCreditCardNumber,
            string paymentInfoCreditCardExpirationDate, string paymentInfoThreeDigitsOnBackOfCard)
        {
            await Task.Delay(5000);
            return checkAns;
        }

        public async Task<bool> Refund(double price, string paymentInfoUserName, string paymentInfoIdNumber, string paymentInfoCreditCardNumber,
            string paymentInfoCreditCardExpirationDate, string paymentInfoThreeDigitsOnBackOfCard)
        {
            await Task.Delay(5000);
            return refundAns;
        }
    }
}