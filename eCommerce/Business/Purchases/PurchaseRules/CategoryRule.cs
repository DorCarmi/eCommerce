using System;
using eCommerce.Business.CombineRules;

namespace eCommerce.Business.PurchaseRules
{
    public class CategoryRule : CompositeRule
    {
        private Category category;
        private Compare<double> compare;

        public CategoryRule(Category category)
        {
            this.category = category;
            this.compare = compare;
        }

        public bool Check(IBasket checkItem1, IUser checkItem2)
        {
            foreach (var item in checkItem1.GetAllItems().Value)
            {
                if (this.category.getName().Equals(item.category))
                {
                    return true;
                }
            }

            return false;
        }
    }
}