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

        public override string ToString()
        {
            return $"Username: {UserName}\nId number: {IDNumber}\nCredit card: {CreditCardNumber}\n" +
                   $"Card expiration date: {CreditCardExpirationDate}\nSecurity digits:{ThreeDigitsOnBackOfCard}\n";
        }
    }
}