using System.Collections.Generic;
using eCommerce.Business.CombineRules;
using eCommerce.Common;

namespace eCommerce.Business.Discounts
{
    public class DiscountInfoLeaf : DiscountInfoNode
    {
        public double theDiscount;
        public RuleInfoNode theRule;

        public DiscountInfoLeaf(double discount, RuleInfoNode rule)
        {
            this.theDiscount = discount;
            this.theRule = rule;
        }

        public override bool hasKids()
        {
            return false;
        }

        public override Result<Composite> handleDiscountInfo()
        {
            return DiscountHandler.Handle(this);
        }
    }
}