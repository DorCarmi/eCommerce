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

        public override string ToString()
        {
            return $"Username: {UserName}\nId number: {IDNumber}\nCredit card: {CreditCardNumber}\n" +
                   $"Card expiration date: {CreditCardExpirationDate}\nSecurity digits:{ThreeDigitsOnBackOfCard}\n";
        }
    }
}