using eCommerce.Business.CombineRules;

namespace eCommerce.Business.PurchaseRules
{
    public class Category : CompositeRule<Category>
    {
        private Category category;
        private Compare<double> compare;

        public Category(Category category)
        {
            this.category = category;
            this.compare = compare;
        }
        

        public bool Check(Category checkItem)
        {
            var x = new Equals<Category>();
            return x.GetResult(this.category, checkItem);
        }
    }
}