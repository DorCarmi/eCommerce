namespace eCommerce.Service.StorePolicies
{
    public class SDiscountNodeLeaf : SDiscountNode
    {
        public SRuleNode Rule { get; set; }

        public SDiscountNodeLeaf(SRuleNode rule) : base(SDiscountNodeType.Leaf)
        {
            Rule = rule;
        }
    }
}