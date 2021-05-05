namespace eCommerce.Business.PurchaseRules
{
    public class Amount : CompositeRule
    {
        
        private int amount;
        private Compare<double> compare;

        public Amount(int age, Compare<double> compare)
        {
            this.amount = age;
            this.compare = compare;
        }


        public bool Check(int checkItem)
        {
            return compare.GetResult(this.amount, checkItem);
        }

        public bool Check(IBasket checkItem1, IUser checkItem2)
        {
            int totalAmount = 0;
            foreach (var item in checkItem1.GetAllItems().Value)
            {
                totalAmount += item.amount;
            }
            return compare.GetResult(this.amount, totalAmount);
        }
    }
}