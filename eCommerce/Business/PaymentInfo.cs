namespace eCommerce.Business
{
    public class PaymentInfo
    {
        public string UserName;
        public string IDNumber;
        public string CreditCardNumber;
        public string CreditCardExpirationDate;
        public string ThreeDigitsOnBackOfCard;

        public string FullAddress;

        public PaymentInfo(string userName, string idNumber, string creditCardNumber, string creditCardExpirationDate, string threeDigitsOnBackOfCard, string fullAddress)
        {
            UserName = userName;
            IDNumber = idNumber;
            CreditCardNumber = creditCardNumber;
            CreditCardExpirationDate = creditCardExpirationDate;
            ThreeDigitsOnBackOfCard = threeDigitsOnBackOfCard;
            FullAddress = fullAddress;
        }
    }
}